open System
open Argu
open Arguments
open FileSyncer


[<EntryPoint>]
let main argv =

    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<FileSyncerArguments>(programName = "DevSync", errorHandler = errorHandler)

    let arguments = (parser.ParseCommandLine argv).GetAllResults()
   
    printfn "Welcome To DevSync"

    let mutable disposables = []

    for arg in arguments do
        match arg with
        | Sync (src, dest) -> match FileSyncer.build src dest with
                              | SourceDirectoryNotFound -> printf "Source Directory %s Not Found" src
                              | DestinationDirectoryNotFound -> printfn "Destination Directory %s Not Found" dest
                              | IsWatching disposable -> printfn "Now watching %s" src
                                                         printfn "             -> %s" dest
                                                         disposables <- (disposable :: disposables)
                                         

    while Console.ReadLine().ToLower() <> "exit" do ()

    for x in disposables do
        try
            x.Dispose()
        with
        | x -> printfn "%A" x

    0