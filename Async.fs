namespace HackerCredTest.Async

open System.Threading.Tasks

[<AutoOpen>]
module Async =
    let inline awaitPlainTask (task: Task) =
        // rethrow exception from preceding task if it faulted
        let continuation (t: Task) : unit =
            match t.IsFaulted with
            | true -> raise t.Exception
            | arg -> ()

        task.ContinueWith continuation |> Async.AwaitTask

    let inline startAsPlainTask (work: Async<unit>) =
        Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)
