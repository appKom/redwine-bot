module Bot.Program

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc
open System.IO
open FSharp.Data

type provider = JsonProvider<""" {"type":"shortcut","token":"xxx","action_ts":"xxx","team":{"id":"xxx","domain":"xxx"},"user":{"id":"xxx","username":"xxx","team_id":"xxx"},"is_enterprise_install":false,"enterprise":null,"callback_id":"getWine","trigger_id":"xxx"} """>

[<FunctionName("ShortcutResponse")>]
let ShortcutResponse
    ([<HttpTrigger()>] request: HttpRequest)
    (logger: ILogger) =
    async {
        let sr = new StreamReader(request.Body)
        let urlDecodedPayload = sr.ReadToEndAsync() |> Async.AwaitTask |> Async.RunSynchronously |> System.Web.HttpUtility.UrlDecode 
        let jsonData = provider.Parse(urlDecodedPayload.Substring 8)
        
        logger.LogWarning(jsonData.Type)  

        return new OkObjectResult("ok")
    }
    |> Async.StartAsTask

