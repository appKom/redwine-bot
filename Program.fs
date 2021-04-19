module Bot.Program

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc
open System.IO
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type provider = JsonProvider<""" {"type":"shortcut","token":"xxx","action_ts":"xxx","team":{"id":"xxx","domain":"xxx"},"user":{"id":"xxx","username":"xxx","team_id":"xxx"},"is_enterprise_install":false,"enterprise":null,"callback_id":"getWine","trigger_id":"xxx"} """>

[<FunctionName("ShortcutResponse")>]
let ShortcutResponse
    ([<HttpTrigger()>] request: HttpRequest)
    (logger: ILogger) =
    async {
        let sr = new StreamReader(request.Body)
        let urlDecodedPayload = sr.ReadToEndAsync() |> Async.AwaitTask |> Async.RunSynchronously |> System.Web.HttpUtility.UrlDecode 
        let jsonData = provider.Parse(urlDecodedPayload.Substring 8)
        let triggerId = jsonData.TriggerId

        logger.LogWarning(jsonData.Type)

   
        // let requestString = (Http.RequestString("https://slack.com/api/views.open", httpMethod = "POST",
        //         headers = [ ContentType HttpContentTypes.Json; Authorization "bearer xoxb-1967817842422-1980766974324-Jb9SjWHejU6mnPG5BaPoqviD" ],
        //         body = TextRequest """ {
        //           "trigger_id": {triggerId},
        //           "view": {
        //             "type": "modal",
        //             "callback_id": "modal-identifier",
        //             "title": {
        //               "type": "plain_text",
        //               "text": "Just a modal"
        //             },
        //             "blocks": [
        //               {
        //                 "type": "section",
        //                 "block_id": "section-identifier",
        //                 "text": {
        //                   "type": "mrkdwn",
        //                   "text": "*Welcome* to ~my~ Block Kit _modal_!"
        //                 },
        //                 "accessory": {
        //                   "type": "button",
        //                   "text": {
        //                     "type": "plain_text",
        //                     "text": "Just a button"
        //                   },
        //                   "action_id": "button-identifier"
        //                 }
        //               }
        //             ]
        //           }
        //         } """ 
        //     ))
                 
        //logger.LogWarning(response.ResponseUrl)

        return new OkObjectResult("ok")
    }
    |> Async.StartAsTask

