open System
open HackerCredTest.Async
open Discord.WebSocket
open Discord
open System.Threading.Tasks
open System.Threading

let token =
    Environment.GetEnvironmentVariable("TOKEN")

let channelId =
    uint64 (Environment.GetEnvironmentVariable("CHANNEL_ID"))

let _client = new DiscordSocketClient()

let mainTask =
    async {
        do!
            _client.LoginAsync(TokenType.Bot, token)
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

    // Handle incoming messages
    _client.add_MessageReceived
        (fun x ->
            async {
                do
                    if (x.Channel.Id = channelId) then
                        Console.WriteLine(x.Content)
                    else
                        ()
            }
            |> startAsPlainTask)

    async {
        do! Async.SwitchToThreadPool()
        return! mainTask
    }
    |> Async.RunSynchronously

    0 // successful exit code
