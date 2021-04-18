module Bot.Program

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc

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
let pong
    ([<TimerTrigger("*/10 * * * * *", RunOnStartup = true)>] timerInfo: TimerInfo)
    (logger: ILogger) =
    async {        
        logger.LogWarning("Pong")
    }
    |> Async.StartAsTask


[<FunctionName("ShortcutResponse")>]
let ShortcutResponse
    ([<HttpTrigger()>] request: HttpRequest)
    (logger: ILogger) =
    async {  
        return new OkObjectResult()
    }
    |> Async.StartAsTask

