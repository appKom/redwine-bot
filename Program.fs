module Bot.Program

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc
open FSharp.Json
open System.IO
[<FunctionName("Ping")>]
let ping
    ([<HttpTrigger()>] request: HttpRequest)
    (logger: ILogger) =
    async {  
        logger.LogWarning("Pong")
        return new OkObjectResult("Pong")
    }
    |> Async.StartAsTask

// [<FunctionName("Pong")>]
//let pong
//    ([<TimerTrigger("*/10 * * * * *", RunOnStartup = true)>] timerInfo: TimerInfo)
//    (logger: ILogger) =
//    async {        
//       logger.LogWarning("Pong")
//    }
//    |> Async.StartAsTask


type teamType = {
    id: string
    domain: string
}

type userType =  {
    id: string
    username: string
    team_id: string
}

type requestType = {
  token: string
  action_ts: string
  team: teamType
  user: userType
  callback_id: string
  trigger_id: string
}

[<FunctionName("ShortcutResponse")>]
let ShortcutResponse
    ([<HttpTrigger()>] request: HttpRequest)
    (logger: ILogger) =
    async {
        let sr = new StreamReader(request.Body)
        let s = sr.ReadToEndAsync() |> Async.AwaitTask |> Async.RunSynchronously
        logger.LogWarning(
           s
        )  
        //let req = Json.deserialize<requestType> 


        return new OkObjectResult("ok")
    }
    |> Async.StartAsTask

