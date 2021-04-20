module Bot.Program

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc
open System.IO
open FSharp.Data

open Hopac
open HttpFs.Client
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
        let triggerId = jsonData.TriggerId

        logger.LogWarning(jsonData.Type)
        let multipartRequest =
            Request.createUrl Post "https://slack.com/api/views.open"
            |> Request.setHeader (ContentType (ContentType.create("application", "json", HttpEncodings.PostDefaultEncoding)))
            |> Request.setHeader (Authorization "Bearer ")
            |> Request.bodyString( "{ trigger_id: " + $""" "{jsonData.TriggerId}", """ + """  
                "view" : {
                  "type": "modal",
                  "title": {
                    "type": "plain_text",
                    "text": "Modal title"
                  },
                  "blocks": [
                    {
                      "type": "section",
                      "text": {
                        "type": "mrkdwn",
                        "text": "It's Block Kit...but _in a modal_"
                      },
                      "block_id": "section1",
                      "accessory": {
                        "type": "button",
                        "text": {
                          "type": "plain_text",
                          "text": "Click me"
                        },
                        "action_id": "button_abc",
                        "value": "Button value",
                        "style": "danger"
                      }
                    },
                    {
                      "type": "input",
                      "label": {
                        "type": "plain_text",
                        "text": "Input label"
                      },
                      "element": {
                        "type": "plain_text_input",
                        "action_id": "input1",
                        "placeholder": {
                          "type": "plain_text",
                          "text": "Type in here"
                        },
                        "multiline": false
                      },
                      "optional": false
                    }
                  ],
                  "close": {
                    "type": "plain_text",
                    "text": "Cancel"
                  },
                  "submit": {
                    "type": "plain_text",
                    "text": "Save"
                  },
                  "private_metadata": "Shhhhhhhh",
                  "callback_id": "view_identifier_12"
                },
            }""")
            |> Request.responseAsString
            |> run

        printfn "Response of get is %s: " multipartRequest

        return new OkObjectResult("ok")
    }
    |> Async.StartAsTask

