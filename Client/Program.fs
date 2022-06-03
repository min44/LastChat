module Program

open Browser
open Elmish
open Elmish.React
open Fable.Core.JS
open Fable.React
open Props

type Model = { State: string
               TextBox: string
               Chat: string list }

type Msg =
    | SentRequest
    | SetTextBox of string
    | SetResponse of string
    | SetChat of string

let webSocket = WebSocket.Create("ws://192.168.68.100:8080/websocket")
let registerOnMessageHandler =
    fun dispatch ->
        async {
            webSocket.addEventListener_message (fun xy -> dispatch <| SetChat (xy.data.ToString()))
        } |> Async.StartImmediate

let init() = { State = "Initial state"
               TextBox = ""
               Chat = [] }, [ registerOnMessageHandler ]
let sendRequest model = fun _ -> async { webSocket.send($"{model.TextBox}") } |> Async.StartImmediate
    
let update msg model =
    match msg with
    | SentRequest    -> model, [ sendRequest model ]
    | SetTextBox v   -> { model with TextBox = v }, Cmd.none
    | SetResponse r  -> { model with State = r }, Cmd.none
    | SetChat x      -> { model with Chat = x::model.Chat }, Cmd.none


let view (model: Model) dispatch =
    div [] [
        div [] [ str model.State ]
        button [ OnClick (fun _ -> dispatch SentRequest) ] [ str "SendRequest" ]
        div [  ] [ input [ Value model.TextBox; OnChange (fun e -> dispatch (SetTextBox e.Value) ) ] ]
        str (model.Chat.ToString()) ]

Program.mkProgram init update view
//|> Program.withSubscription onMessage 
|> Program.withReactSynchronous "elmish-app"
|> Program.run