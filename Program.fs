module Program

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting

[<EntryPoint>]
let main argv =

    WebHostBuilder()
        .UseKestrel()
        .UseUrls([|"http://localhost:5000"|])
        .Configure(fun b ->
            // OWIN MidFunc
            b.UseOwin(fun p -> p.Invoke App.middleware) |> ignore
            // OWIN AppFunc
            b.UseOwin(fun p -> p.Invoke (fun _ -> App.app)) |> ignore)
        .Build()
        .Run()

    0
