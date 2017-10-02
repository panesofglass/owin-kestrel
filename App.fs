module App

open System
open System.Collections.Generic
open System.Threading.Tasks

type OwinEnv = IDictionary<string, obj>
type OwinAppFunc = Func<OwinEnv, Task>
type OwinMidFunc = Func<OwinAppFunc, OwinAppFunc>

let middleware = OwinMidFunc(fun next -> OwinAppFunc(fun env ->
    let path = unbox<string> env.["owin.RequestPath"]
    if path.StartsWith "/hello" then
        let httpMethod = unbox<string> env.["owin.RequestMethod"]
        if httpMethod = "GET" then
            let parts = path.Split([|'/'|], 3)
            let name =
                match parts with
                | [|_;"hello";name|] -> name
                | _ -> "World"
            let content =
                Text.Encoding.UTF8.GetBytes(sprintf "Hello, %s" name)

            env.["owin.ResponseStatusCode"] <- box 200
            env.["owin.ResponseReasonPhrase"] <- box "OK"

            let headers = unbox<IDictionary<string, string[]>> env.["owin.ResponseHeaders"]
            headers.["Content-Type"] <- [|"text/plain"|]
            headers.["Content-Length"] <- [|string content.Length|]

            let body = unbox<IO.Stream> env.["owin.ResponseBody"]
            body.WriteAsync(content, 0, content.Length)

        else
            env.["owin.ResponseStatusCode"] <- box 405
            env.["owin.ResponseReasonPhrase"] <- box "Method not allowed"
            Task.FromResult() :> Task

    else next.Invoke env
))

let app = OwinAppFunc(fun env ->
    let content =
        Text.Encoding.UTF8.GetBytes(sprintf "Hello from OWIN!")

    env.["owin.ResponseStatusCode"] <- box 200
    env.["owin.ResponseReasonPhrase"] <- box "OK"

    let headers = unbox<IDictionary<string, string[]>> env.["owin.ResponseHeaders"]
    headers.["Content-Type"] <- [|"text/plain"|]
    headers.["Content-Length"] <- [|string content.Length|]

    let body = unbox<IO.Stream> env.["owin.ResponseBody"]
    body.WriteAsync(content, 0, content.Length)
)
