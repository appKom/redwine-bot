import logging
import requests
import jinja2 as j
from urllib.parse import unquote
import json
from os import environ

import azure.functions as func

def main(req: func.HttpRequest) -> func.HttpResponse:
  json_req_body = json.loads(unquote(req.get_body()).removeprefix('payload='))
  triggerId = json_req_body['trigger_id']

  jenv = j.Environment(loader=j.FileSystemLoader("./request_templates"))
  template = jenv.get_template("redwine_menu.json.jinja")

  formData = template.render(triggerid=triggerId).encode('utf-8')

  postRes = requests.post(
      "https://slack.com/api/views.open",
      data=formData,
      headers={
          'Content-Type': 'application/json; charset=UTF-8',
          'Authorization': f'Bearer {environ["BotToken"]}',
      },
  )

  return func.HttpResponse(status_code=requests.codes.ok)