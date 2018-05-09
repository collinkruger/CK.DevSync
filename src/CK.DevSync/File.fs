module File

open System
open System.IO

// TODO: Domain model this properly
let copyRel dirSrc dirDest (fpSrc: string) =
    if not (fpSrc.StartsWith(dirSrc, StringComparison.OrdinalIgnoreCase)) then
        failwith (sprintf "fpSrc (%s) must start with dirSrc (%s). It does not. That is super weird." fpSrc dirSrc)
    else
        let fpDest = dirDest + (fpSrc.Substring (dirSrc.Length))
        File.Copy(fpSrc, fpDest, true)


// TODO: Domain model this properly
let deleteRel dirSrc dirDest (fpSrc: string) =
    if not (fpSrc.StartsWith(dirSrc, StringComparison.OrdinalIgnoreCase)) then
        failwith (sprintf "fpSrc (%s) must start with dirSrc (%s). It does not. That is super weird." fpSrc dirSrc)
    else
        let fpDest = dirDest + (fpSrc.Substring (dirSrc.Length))
        File.Delete fpDest