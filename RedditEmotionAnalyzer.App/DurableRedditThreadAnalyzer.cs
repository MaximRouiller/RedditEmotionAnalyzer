using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedditEmotionAnalyzer.App.Model;

namespace RedditEmotionAnalyzer.App
{
    public static class DurableRedditThreadAnalyzer
    {
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(Environment.GetEnvironmentVariable("CognitiveServices_Key", EnvironmentVariableTarget.Process));
        private static readonly Uri endpoint = new Uri(Environment.GetEnvironmentVariable("CognitiveServices_Endpoint", EnvironmentVariableTarget.Process));
        private static readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);

        [FunctionName("RedditThreadAnalyzer")]
        public static async Task<RedditEmotionResult> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            string redditJsonUrl = context.GetInput<string>();

            var existingResult = await context.CallActivityAsync<RedditEmotionResult>("RedditThreadAnalyzer_GetExistingEmotionResult", redditJsonUrl);
            if(existingResult != null)
            {
                return existingResult;
            }

            var jsonContent = await context.CallHttpAsync(HttpMethod.Get, new Uri(redditJsonUrl));
            var comments = await context.CallActivityAsync<List<(string id, string comment)>>("RedditThreadAnalyzer_ParseAllComments", jsonContent.Content);
            var redditEmotionResult = await context.CallActivityAsync<RedditEmotionResult>("RedditThreadAnalyzer_AnalyzeEmotions", comments);
            await context.CallActivityAsync("RedditThreadAnalyzer_SaveResults", new SaveResultInput { EmotionResult = redditEmotionResult, Url = redditJsonUrl });
            return redditEmotionResult;
        }

        [FunctionName("RedditThreadAnalyzer_GetExistingEmotionResult")]
        public static async Task<RedditEmotionResult> GetExistingEmotionResult([ActivityTrigger] string url)
        {
            CloudStorageAccount account;
            CloudStorageAccount.TryParse(storageConnectionString, out account);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("AnalyzedRedditThread");
            await table.CreateIfNotExistsAsync();

            return table.CreateQuery<AnalyzedRedditThreadEntity>().Where(x => x.RowKey == HashString(url)).Select(x => new RedditEmotionResult
            {
                Positive = (decimal)x.Positive,
                Negative = (decimal)x.Negative,
                Neutral = (decimal)x.Neutral,
                Mixed = (decimal)x.Mixed
            }).SingleOrDefault();
        }

        [FunctionName("RedditThreadAnalyzer_SaveResults")]
        public static async Task SaveResults([ActivityTrigger] SaveResultInput result)
        {
            CloudStorageAccount account;
            CloudStorageAccount.TryParse(storageConnectionString, out account);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("AnalyzedRedditThread");
            await table.CreateIfNotExistsAsync();

            var entity = new AnalyzedRedditThreadEntity();
            entity.PartitionKey = "default";
            entity.RowKey = HashString(result.Url);
            entity.Url = result.Url;
            entity.Positive = (double)result.EmotionResult.Positive;
            entity.Negative = (double)result.EmotionResult.Negative;
            entity.Neutral = (double)result.EmotionResult.Neutral;
            entity.Mixed = (double)result.EmotionResult.Mixed;
            entity.ProcessedDate = DateTime.UtcNow;

            var operation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(operation);
        }

        private static string HashString(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return HttpUtility.UrlEncode(Convert.ToBase64String(bytes));
        }

        [FunctionName("RedditThreadAnalyzer_ParseAllComments")]
        public static async Task<List<(string id, string comment)>> ParseAllComments([ActivityTrigger] string jsonContent)
        {
            var listings = JsonConvert.DeserializeObject<List<Listing>>(jsonContent);

            List<(string id, string comment)> comments = new List<(string id, string comment)>();
            foreach (var child in from listing in listings
                                  from child in listing.data.children
                                  where child.IsComment
                                  select child)
            {
                await child.data.AddCommentToList(comments);
            }

            return comments;
        }

        [FunctionName("RedditThreadAnalyzer_AnalyzeEmotions")]
        public static async Task<RedditEmotionResult> AnalyzeEmotionsAsync([ActivityTrigger] List<(string id, string comment)> comments, ILogger log)
        {
            var commentsToAnalyze = comments.Select(c => new TextDocumentInput(c.id, c.comment)).ToList();
            TextAnalyticsClient textAnalyticClient = new TextAnalyticsClient(endpoint, credentials);
            IEnumerable<string> sentimentResult = (await textAnalyticClient.AnalyzeSentimentBatchAsync(commentsToAnalyze)).Value.Select(x => x.DocumentSentiment.Sentiment.ToString()).ToList();

            Func<string, decimal> percentage = (sentiment) => sentimentResult.Where(x => x == sentiment).Count() / (decimal)commentsToAnalyze.Count * 100;

            return new RedditEmotionResult
            {
                Positive = percentage("Positive"),
                Negative = percentage("Negative"),
                Neutral = percentage("Neutral"),
                Mixed = percentage("Mixed")
            };
        }

        [FunctionName("RedditThreadAnalyzer_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var client = HttpClientFactory.Create();
            var query = HttpUtility.ParseQueryString(req.RequestUri.Query);
            var url = new Uri(HttpUtility.UrlDecode(query.Get("url")));

            var postJsonUrl = url.AbsoluteUri + ".json";

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("RedditThreadAnalyzer", input: postJsonUrl);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}