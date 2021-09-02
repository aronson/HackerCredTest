open System
open System.Threading.Tasks

let mainTask = async { () }

[<EntryPoint>]
let main argv =
    async {
        do! Async.SwitchToThreadPool()
        return! mainTask
    }
    |> Async.RunSynchronously

    0 // successful exit code
