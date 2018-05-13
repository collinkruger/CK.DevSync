module TutorialWebSocket

open System
open Suave
open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket

let ws (webSocket : WebSocket) (context: HttpContext) =
  socket {
    // if `loop` is set to false, the server will stop receiving messages
    let mutable loop = true

    while loop do
      // the server will wait for a message to be received without blocking the thread
      let! msg = webSocket.read()

      match msg with
      // the message has type (Opcode * byte [] * bool)
      //
      // Opcode type:
      //   type Opcode = Continuation | Text | Binary | Reserved | Close | Ping | Pong
      //
      // byte [] contains the actual message
      //
      // the last element is the FIN byte, explained later
      | (Text, data, true) ->
        // the message can be converted to a string
        let str = UTF8.toString data
        let response = sprintf "response to %s" str

        // the response needs to be converted to a ByteSegment
        let byteResponse =
          response
          |> System.Text.Encoding.ASCII.GetBytes
          |> ByteSegment

        // the `send` function sends a message back to the client
        do! webSocket.send Text byteResponse true

      | (Close, _, _) ->
        let emptyResponse = [||] |> ByteSegment
        do! webSocket.send Close emptyResponse true

        // after sending a Close message, stop the loop
        loop <- false

      | _ -> ()
    }

/// An example of explictly fetching websocket errors and handling them in your codebase.
let wsWithErrorHandling (webSocket : WebSocket) (context: HttpContext) = 
   
   let exampleDisposableResource = { new IDisposable with member __.Dispose() = printfn "Resource needed by websocket connection disposed" }
   let websocketWorkflow = ws webSocket context
   
   async {
    let! successOrError = websocketWorkflow
    match successOrError with
    // Success case
    | Choice1Of2() -> ()
    // Error case
    | Choice2Of2(error) ->
        // Example error handling logic here
        printfn "Error: [%A]" error
        exampleDisposableResource.Dispose()
        
    return successOrError
   }

type Message =
    | FromServer of string
    | FromClient of Opcode * ByteSegment * bool

let genEmptyInbox () = MailboxProcessor<Message>.Start (fun _ -> async { () })

let mutable inboxHandle = genEmptyInbox()

let genInbox (ws:WebSocket) =
    inboxHandle <- MailboxProcessor.Start (fun inbox -> async {
        while true do
            let! message = inbox.Receive()
            match message with
            | FromServer str ->            let! _ = ws.send Text (str |> System.Text.Encoding.ASCII.GetBytes |> ByteSegment) true
                                           ()
            | FromClient (op, seg, fin) -> inboxHandle <- genEmptyInbox()
    })
    inboxHandle

let handleWebsocketConnection (ws: WebSocket) (context: HttpContext) =
        let mutable loop = true

        let inbox = genInbox ws

        let timer = new System.Timers.Timer(50.)
        timer.Elapsed.Add (fun _ -> inbox.Post (FromServer "hi"))
        timer.Start()

        socket {
            while loop do
                let! m = ws.read()
                match m with
                | Text, data, true -> inbox.Post (FromClient (Text, "Yo dog" |> System.Text.Encoding.ASCII.GetBytes |> ByteSegment, true))
                | Ping, _ ,_ ->       inbox.Post (FromClient (Pong, ByteSegment.Empty, true))
                | Close, _ ,_ ->      inbox.Post (FromClient (Close, ByteSegment.Empty, true))
                                      loop <- false
                | _ -> ()
        }