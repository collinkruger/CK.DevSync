open System
open Argu
open Arguments


[<EntryPoint>]
let main argv =

    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<FileSyncerArguments>(programName = "DevSync", errorHandler = errorHandler)

    let arguments = (parser.ParseCommandLine argv).GetAllResults()
   
    printfn "Welcome To DevSync"

    for arg in arguments do
        match arg with
        | Sync (src, dest) -> FileSyncer.watch src dest
                              printfn "Now watching %s" src
                              printfn "             -> %s" dest

    Console.ReadKey() |> ignore

    //handle.Dispose()

    0