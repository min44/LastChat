module Server

open System.IO
open Suave
open Suave.Sockets.Control
open Suave.Utils
open Filters
open Operators
open WebSocket

let ws (webSocket : WebSocket) _ =
    socket {
        let! _, input, _ = webSocket.read()
        let text = ASCII.toString input
        printfn $"{text}" }
  
let app: WebPart =
     choose [
          GET >=> path "/" >=> Files.file "./public/index.html"    
          GET >=> path "/test" >=> Successful.OK "Biden666"
          GET >=> Files.browseHome
          path "/websocket" >=> handShake ws
          RequestErrors.NOT_FOUND "Page not found." ]

let config = { defaultConfig with homeFolder = Some(Path.GetFullPath "./public") }

startWebServer config app