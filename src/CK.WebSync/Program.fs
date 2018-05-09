// Learn more about F# at http://fsharp.org

open System
open System.Reflection
open System.IO

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"

    use stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CK.WebSync.Script.js");
    use reader = new StreamReader(stream)

    Console.Write(reader.ReadToEnd())

    Console.ReadLine() |> ignore

    0