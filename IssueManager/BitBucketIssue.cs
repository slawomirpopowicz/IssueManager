namespace IssueManager
{
    public class BitBucketIssue
    {        
        public int id { get; set; }
        public string title { get; set; }
        public Content content { get; set; }
        public string state { get; set; }
    }

    public class Content
    {
        public string raw { get; set; }
    }
}
