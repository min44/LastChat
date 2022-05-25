module Program

open Browser
open Elmish
open Elmish.React
open Fable.React
open Props

type Model = { State: string }

type Msg =
    | SentRequest
    | SetResponse of string

let init() = { State = "Initial state" }, []
let webSocket = WebSocket.Create("ws://127.0.0.1:8080/websocket")
let sendRequest  = fun _ -> async { webSocket.send("Hello!") } |> Async.StartImmediate
    
let update msg model =
    match msg with
    | SentRequest   -> model, [ sendRequest ]
    | SetResponse r -> { model with State = r }, Cmd.none

let view (model: Model) dispatch =
    div [] [
        div [] [ str model.State ]
        button [ OnClick (fun _ -> dispatch SentRequest) ] [ str "SendRequest" ] ]

Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run