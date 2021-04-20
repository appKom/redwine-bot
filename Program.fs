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
              	"submit": {
              		"type": "plain_text",
              		"text": "Submit",
              		"emoji": true
              	},
              	"close": {
              		"type": "plain_text",
              		"text": "Cancel",
              		"emoji": true
              	},
              	"title": {
              		"type": "plain_text",
              		"text": "Redwine",
              		"emoji": true
              	},
              	"blocks": [
              		{
              			"type": "header",
              			"text": {
              				"type": "plain_text",
              				"text": "Legg til straff!",
              				"emoji": true
              			}
              		},
              		{
              			"type": "section",
              			"text": {
              				"type": "mrkdwn",
              				"text": "Velg bruker"
              			},
              			"accessory": {
              				"type": "users_select",
              				"placeholder": {
              					"type": "plain_text",
              					"text": "Select a user",
              					"emoji": true
              				},
              				"action_id": "users_select-action"
              			}
              		},
              		{
              			"type": "actions",
              			"elements": [
              				{
              					"type": "button",
              					"text": {
              						"type": "plain_text",
              						"text": "🍺",
              						"emoji": true
              					},
              					"value": "click_me_123"
              				},
              				{
              					"type": "button",
              					"text": {
              						"type": "plain_text",
              						"text": "🍷",
              						"emoji": true
              					},
              					"value": "click_me_123"
              				},
              				{
              					"type": "button",
              					"text": {
              						"type": "plain_text",
              						"text": "🍸",
              						"emoji": true
              					},
              					"value": "click_me_123"
              				}
              			]
              		},
              		{
              			"type": "divider"
              		},
              		{
              			"type": "input",
              			"element": {
              				"type": "plain_text_input",
              				"multiline": true,
              				"action_id": "plain_text_input-action"
              			},
              			"label": {
              				"type": "plain_text",
              				"text": "Begrunnelse",
              				"emoji": true
              			}
              		}
              	]
              }
            }""")
            |> Request.responseAsString
            |> run

        printfn "Response of get is %s: " multipartRequest

        return new OkObjectResult("ok")
    }
    |> Async.StartAsTask

