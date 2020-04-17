using System;
using Microsoft.Azure.Cosmos.Table;

namespace RedditEmotionAnalyzer.App
{
    public class AnalyzedRedditThreadEntity : TableEntity
    {
        public string Url { get; set; }
        public DateTime ProcessedDate { get; set; }
        public double Positive { get; set; }
        public double Negative { get; set; }
        public double Neutral { get; set; }
        public double Mixed { get; set; }
    }
}