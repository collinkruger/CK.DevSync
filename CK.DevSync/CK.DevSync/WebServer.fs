module FileServer

open Suave
open Suave.Successful
open Suave.Filters
open Suave.Operators

let webServer () =
    startWebServer defaultConfig (choose [ path "/js" >=> OK  "JavaScript"
                                           path "/html" >=> OK "HTML" ])