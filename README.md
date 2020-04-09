# Reddit Thread Emotion Analyzer

This sample requests the `JSON` file related to a reddit thread by doing an HTTP request on a Reddit URL and appending `.json` at the end of it.

This JSON file contains data about the thread as well as the first 200 comments of a thread.

## Prerequisites

You will need the following:

* Latest LTS of .NET Core
* Azure Account (free trial available)
* Create a Cognitive Services Text Analytics service
* Creating a `local.settings.json` file
    * `CognitiveServices_Key` environment variable containing the Cognitive Services Key
    * `CognitiveServices_Endpoint` environment variable containing the Cognitive Services Endpoint

## Creating a Text Analytics Service

This can be done by simply clicking the button below and filling the required fields.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMaximRouiller%2FOneClickCognitiveServices%2Fmaster%2Fcognitiveservices%2FTextAnalytics.json)

## Build

We need to first clone the repository.

```bash
git clone https://github.com/MaximRouiller/RedditEmotionAnalyzer
```

### With Visual Studio

Open up `RedditEmotionAnalyzer.App\RedditEmotionAnalyzer.App.sln` by double clicking on it.

Use the menu `Build > Build Solution` and the solution will compile and be ready for use.

Use the `Debug > Start Debugging` to launch the Azure Functions process

### With Code

Open up the folder `RedditEmotionAnalyzer.App`.

Use the menu `Terminal > Run Build Task...` and the solution will compile and be ready for use.

Use the `Run > Start Debugging` to launch the Azure Functions process

## Running an example

Once your application is launched, you just need to open up a browser and use a Reddit thread in the `url` parameter.

The following example return the results immediately. 

```none
http://localhost:7071/api/AnalyzeRedditThread?url=<URL>

http://localhost:7071/api/AnalyzeRedditThread?url=https://www.reddit.com/r/dotnet/comments/ftsjhp/what_did_you_all_do_this_week/
```

This example returns a URL on which to check the status of the request. This uses Azure Durable functions.

```none
http://localhost:7071/api/RedditThreadAnalyzer_HttpStart?url=<URL>

http://localhost:7071/api/RedditThreadAnalyzer_HttpStart?url=https://www.reddit.com/r/dotnet/comments/ftsjhp/what_did_you_all_do_this_week/
```

It returns the following payload.

```json
{
    "id": "1ce610e87165442d88895b23a3f756ec",
    "statusQueryGetUri": "http://localhost:7071/runtime/webhooks/durabletask/instances/1ce610e87165442d88895b23a3f756ec?taskHub=TestHubName&connection=Storage&code=uWgiCct6AA/mmvrY/hNE38V/vcrdMDaKypT3FItIuzvh95bQFCTYuA==",
    "sendEventPostUri": "...",
    "terminatePostUri": "...",
    "purgeHistoryDeleteUri": "..."
}
```

Querying the `statusQueryGetUri` from the browser will return the status of the processing. Once completed, we're going to have the following payload.

```json
{
    "name": "RedditThreadAnalyzer",
    "instanceId": "eef70ba78ee14cdeaa0330de4a7c85ce",
    "runtimeStatus": "Completed",
    "input": "https://www.reddit.com/r/dotnet/comments/ftsjhp/what_did_you_all_do_this_week/.json",
    "customStatus": null,
    "output": {
        "Positive": 31.506849315068493,
        "Negative": 17.80821917808219,
        "Neutral": 27.397260273972602,
        "Mixed": 23.28767123287671
    },
    "createdTime": "2020-04-08T12:46:46Z",
    "lastUpdatedTime": "2020-04-08T12:46:52Z"
}
```