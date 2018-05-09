module Arguments

open Argu


[<NoAppSettings>]
type FileSyncerArguments =
    | [<MainCommand>] Sync of string * string
with
    interface IArgParserTemplate with
        member arg.Usage =
            match arg with
            | Sync (_, _) -> "The Source Directory Path and Destination Directory Path of a sync"