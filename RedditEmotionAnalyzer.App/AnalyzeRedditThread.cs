using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using RedditEmotionAnalyzer.App.Model;
using Azure.AI.TextAnalytics;
using Azure;

namespace RedditEmotionAnalyzer.App
{
    public static class AnalyzeRedditThread
    {
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(Environment.GetEnvironmentVariable("CognitiveServices_Key", EnvironmentVariableTarget.Process));
        private static readonly Uri endpoint = new Uri(Environment.GetEnvironmentVariable("CognitiveServices_Endpoint", EnvironmentVariableTarget.Process));

        [FunctionName("AnalyzeRedditThread")]
        public static async Task<IActionResult> AnalyzeThread(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var client = HttpClientFactory.Create();
            var url = new Uri(req.Query["url"].FirstOrDefault());

            var postJson = await client.GetStringAsync(url.AbsoluteUri + ".json");
            var listings = JsonConvert.DeserializeObject<List<Listing>>(postJson);

            var comments = new List<(string id, string comment)>();
            foreach (var child in from listing in listings
                                  from child in listing.data.children
                                  where child.IsComment
                                  select child)
            {
                await AddCommentToList(comments, child.data);
            }

            log.LogInformation($"Found a total of '{comments.Count}' comments.");

            var commentsToAnalyze = comments.Select(c => new TextDocumentInput(c.id, c.comment)).ToList();
            TextAnalyticsClient textAnalyticClient = new TextAnalyticsClient(endpoint, credentials);
            IEnumerable<string> sentimentResult = (await textAnalyticClient.AnalyzeSentimentBatchAsync(commentsToAnalyze)).Value.Select(x => x.DocumentSentiment.Sentiment.ToString());

            Func<string, decimal> percentage = (sentiment) => sentimentResult.Where(x => x == sentiment).Count() / (decimal)commentsToAnalyze.Count * 100;

            var positiveString = $"{Math.Round(percentage("Positive"), 1)}% positive";
            var negativeString = $"{Math.Round(percentage("Negative"), 1)}% negative";
            var neutralString = $"{Math.Round(percentage("Neutral"), 1)}% neutral";
            var mixedString = $"{Math.Round(percentage("Mixed"), 1)}% mixed";

            return new OkObjectResult($"For URL {url}\r\n{comments.Count} comments analyzed with {positiveString}, {negativeString}, {neutralString}, and {mixedString}.");
        }


        private static async Task AddCommentToList(List<(string id, string comment)> comments, Data1 data)
        {
            comments.Add((id: data.id, comment: data.body));

            if (data.replies != null)
            {
                foreach (var comment in data.replies?.data.children)
                {
                    if (comment.IsComment)
                    {
                        await AddCommentToList(comments, comment.data);
                    }
                }
            }
        }
    }
}
