using System.Collections.Generic;
using System.Threading.Tasks;
using RedditEmotionAnalyzer.App.Model;

namespace RedditEmotionAnalyzer.App
{
    public static class Data1Extension
    {
        public static async Task AddCommentToList(this Data1 data, List<(string id, string comment)> comments)
        {
            comments.Add((id: data.id, comment: data.body));

            if (data.replies == null) return;

            foreach (var comment in data.replies?.data.children)
            {
                if (comment.IsComment)
                {
                    await comment.data.AddCommentToList(comments);
                }
            }
        }

    }
}