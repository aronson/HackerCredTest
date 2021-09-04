open System
open System.Linq
open HackerCredTest.Async
open Discord.WebSocket
open Discord
open System.Threading.Tasks
open System.Threading

let token =
    Environment.GetEnvironmentVariable("TOKEN")

let guildId =
    uint64 (Environment.GetEnvironmentVariable("GUILD"))

let channelId =
    uint64 (Environment.GetEnvironmentVariable("CHANNEL"))

let roleName =
    Environment.GetEnvironmentVariable("ROLE")

// Generate SHA512 hash of input string
let generateHash text =
    let getbytes: (string -> byte []) = System.Text.Encoding.UTF8.GetBytes

    use algorithm =
        new Security.Cryptography.SHA512Managed()

    text
    |> (getbytes
        >> algorithm.ComputeHash
        >> System.Convert.ToHexString)

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
                if (x.Channel.Id = channelId) then
                    let username =
                        x.Author.Username + "#" + x.Author.Discriminator

                    let hashedUsername = generateHash username
                    let normalizedMessage = x.Content.ToUpper().Trim()
                    do Console.WriteLine($"Hashed username string is: {hashedUsername}")
                    do Console.WriteLine($"Received content: {normalizedMessage}")

                    if (hashedUsername = normalizedMessage) then
                        do Console.WriteLine($"User {username} passed the test, assigning role")
                        let guild = _client.GetGuild(guildId)

                        let role =
                            guild.Roles.First(fun x -> x.Name = roleName)

                        let user = guild.GetUser(x.Author.Id)
                        do! user.AddRoleAsync(role) |> awaitPlainTask

                    do! x.Channel.DeleteMessageAsync(x) |> awaitPlainTask
            }
            |> startAsPlainTask)

    async {
        do! Async.SwitchToThreadPool()
        return! mainTask
    }
    |> Async.RunSynchronously

    0 // successful exit code
