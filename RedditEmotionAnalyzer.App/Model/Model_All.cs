namespace RedditEmotionAnalyzer.App.Model
{
    public class Listing
    {
        public string kind { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string modhash { get; set; }
        public int? dist { get; set; }
        public Child[] children { get; set; }
        public object after { get; set; }
        public object before { get; set; }
    }

    public class Child
    {
        public string kind { get; set; }
        public Data1 data { get; set; }

        public bool IsComment => kind == "t1";
        public bool IsPost => kind == "t3";
    }

    public class Data1
    {
        public object approved_at_utc { get; set; }
        public string subreddit { get; set; }
        public string selftext { get; set; }
        public object[] user_reports { get; set; }
        public bool saved { get; set; }
        public object mod_reason_title { get; set; }
        public int gilded { get; set; }
        public bool clicked { get; set; }
        public string title { get; set; }
        public object[] link_flair_richtext { get; set; }
        public string subreddit_name_prefixed { get; set; }
        public bool hidden { get; set; }
        public int pwls { get; set; }
        public object link_flair_css_class { get; set; }
        public int downs { get; set; }
        public int thumbnail_height { get; set; }
        public string parent_whitelist_status { get; set; }
        public bool hide_score { get; set; }
        public string name { get; set; }
        public bool quarantine { get; set; }
        public string link_flair_text_color { get; set; }
        public float upvote_ratio { get; set; }
        public string author_flair_background_color { get; set; }
        public string subreddit_type { get; set; }
        public int ups { get; set; }
        public int total_awards_received { get; set; }
        public int thumbnail_width { get; set; }
        public string author_flair_template_id { get; set; }
        public bool is_original_content { get; set; }
        public string author_fullname { get; set; }
        public object secure_media { get; set; }
        public bool is_reddit_media_domain { get; set; }
        public bool is_meta { get; set; }
        public object category { get; set; }
        public object link_flair_text { get; set; }
        public bool can_mod_post { get; set; }
        public int num_duplicates { get; set; }
        public object approved_by { get; set; }
        public bool author_premium { get; set; }
        public string thumbnail { get; set; }
        public bool edited { get; set; }
        public string author_flair_css_class { get; set; }
        public string post_hint { get; set; }
        public object content_categories { get; set; }
        public bool is_self { get; set; }
        public object mod_note { get; set; }
        public float created { get; set; }
        public string link_flair_type { get; set; }
        public int wls { get; set; }
        public object removed_by_category { get; set; }
        public object banned_by { get; set; }
        public string author_flair_type { get; set; }
        public string domain { get; set; }
        public bool allow_live_comments { get; set; }
        public object selftext_html { get; set; }
        public object likes { get; set; }
        public object suggested_sort { get; set; }
        public object banned_at_utc { get; set; }
        public object view_count { get; set; }
        public bool archived { get; set; }
        public int score { get; set; }
        public bool no_follow { get; set; }
        public bool is_crosspostable { get; set; }
        public bool pinned { get; set; }
        public bool over_18 { get; set; }
        public object[] all_awardings { get; set; }
        public object[] awarders { get; set; }
        public bool media_only { get; set; }
        public bool can_gild { get; set; }
        public bool spoiler { get; set; }
        public bool locked { get; set; }
        public string author_flair_text { get; set; }
        public bool visited { get; set; }
        public object removed_by { get; set; }
        public object num_reports { get; set; }
        public string distinguished { get; set; }
        public string subreddit_id { get; set; }
        public object mod_reason_by { get; set; }
        public object removal_reason { get; set; }
        public string link_flair_background_color { get; set; }
        public string id { get; set; }
        public bool is_robot_indexable { get; set; }
        public object report_reasons { get; set; }
        public string author { get; set; }
        public object discussion_type { get; set; }
        public int num_comments { get; set; }
        public bool send_replies { get; set; }
        public object media { get; set; }
        public bool contest_mode { get; set; }
        public bool author_patreon_flair { get; set; }
        public string author_flair_text_color { get; set; }
        public string permalink { get; set; }
        public string whitelist_status { get; set; }
        public bool stickied { get; set; }
        public string url { get; set; }
        public int subreddit_subscribers { get; set; }
        public float created_utc { get; set; }
        public int num_crossposts { get; set; }
        public object[] mod_reports { get; set; }
        public bool is_video { get; set; }
        public string link_id { get; set; }
        public Listing replies { get; set; }
        public string parent_id { get; set; }
        public string body { get; set; }
        public bool is_submitter { get; set; }
        public string body_html { get; set; }
        public string collapsed_reason { get; set; }
        public object associated_award { get; set; }
        public bool score_hidden { get; set; }
        public bool collapsed { get; set; }
        public int controversiality { get; set; }
        public int depth { get; set; }
        public object collapsed_because_crowd_control { get; set; }
    }

}
