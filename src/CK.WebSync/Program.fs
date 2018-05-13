open System.Reflection
open System.IO
open Suave
open Suave.Operators
open Suave.Filters
open Suave.Writers


let getResource key (assembly: Assembly) =
    use stream = assembly.GetManifestResourceStream(key)
    use reader = new StreamReader(stream)
    reader.ReadToEnd()


[<EntryPoint>]
let main argv =
    printfn "Welcome To WebSync!"

    let assembly = Assembly.GetExecutingAssembly()

    let js = getResource "CK.WebSync.Script.js" assembly
    let exampleHTML = getResource "CK.WebSync.Example.html" assembly

    startWebServer defaultConfig (choose [ GET >=> path "/js" >=> Successful.OK js >=> setMimeType "application/javascript; charset=utf-8"
                                           GET >=> path "/example" >=> Successful.OK exampleHTML >=> setMimeType "text/html; charset=utf-8"
                                           GET >=> path "/ws" >=> WebSocket.handShake (TutorialWebSocket.handleWebsocketConnection) ])

    0