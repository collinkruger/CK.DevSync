module Observable

open System
open FSharp.Control.Reactive

let effect (func: 'T -> unit) (src: IObservable<'T>) =
    Observable.map (fun x -> func x; x) src