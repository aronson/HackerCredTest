open System
open HackerCredTest.Async
open Discord.WebSocket
open Discord
open System.Threading.Tasks
open System.Threading

let _client = new DiscordSocketClient()

let mainTask =
    async {
        do!
            _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"))
            |> awaitPlainTask

        do! _client.StartAsync() |> awaitPlainTask

        while true do
            do! Task.Delay(Timeout.Infinite) |> awaitPlainTask
    }

[<EntryPoint>]
let main _ =

    // Add basic logging
    _client.add_Log
        (fun x ->
            async { do Console.WriteLine(x) }
            |> startAsPlainTask)

    async {
        do! Async.SwitchToThreadPool()
        return! mainTask
    }
    |> Async.RunSynchronously

    0 // successful exit code
