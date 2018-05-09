module FileSyncer

open System
open System.IO
open FSharp.Control
open FSharp.Control.Reactive


let private dump (str: string) (args: FileSystemEventArgs) =
    let print str (obj:'T) = printfn "%s | %A" str obj
    
    let w = (String.replicate (str.Length) " ")
    
    print str args.ChangeType
    print w   args.FullPath
    print w   args.Name


let private handleEvent dirSrc dirDest (args: FileSystemEventArgs) =
    match args.ChangeType with
    | WatcherChangeTypes.Created
    | WatcherChangeTypes.Changed -> File.copyRel dirSrc dirDest (args.FullPath)
    | WatcherChangeTypes.Deleted -> File.deleteRel dirSrc dirDest (args.FullPath)
    | WatcherChangeTypes.All
    | WatcherChangeTypes.Renamed
    | _                          -> ()


let watch dirSrc dirDest =

    let fsw = new FileSystemWatcher()

    fsw.Path <- dirSrc
    fsw.NotifyFilter <- NotifyFilters.FileName ||| NotifyFilters.LastWrite
    fsw.IncludeSubdirectories <- true
    
    Observable.merge (fsw.Changed) (fsw.Created)
    |> Observable.merge (fsw.Deleted)
    |> Observable.throttle (TimeSpan.FromSeconds 0.1)
    |> Observable.effect (dump "FileSystemEventArgs")
    |> Observable.add (handleEvent dirSrc dirDest)

    fsw.EnableRaisingEvents <- true

    { new System.IDisposable with member __.Dispose() = fsw.Dispose() }


type BuildResult =
    | IsWatching of IDisposable
    | SourceDirectoryNotFound
    | DestinationDirectoryNotFound


let build dirSrc dirDest =
    let trim (str: string) = str.TrimEnd('\\').TrimEnd('/');

    let diSrc = DirectoryInfo(trim dirSrc);
    let diDest = DirectoryInfo(trim dirDest);

    match (diSrc.Exists), (diDest.Exists) with
    | false, _   -> SourceDirectoryNotFound
    | _, false   -> DestinationDirectoryNotFound
    | true, true -> IsWatching (watch (diSrc.FullName) (diDest.FullName))