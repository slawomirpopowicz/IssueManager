namespace IssueManager
{
    public class Issue: IComparable
    {
        private string description;
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description
        {
            get { return description; }
            set { if (!String.IsNullOrEmpty(value))
                    description = value;
                else
                    description = "";
                }
        }

        public IssueState State { get; set; } = IssueState.open;

        public Issue()
        {            
        }

        public Issue(GitHubIssue gitHubIssue)
        {
            Id = gitHubIssue.number;
            Title = gitHubIssue.title;
            Description = gitHubIssue.body;
            State = gitHubIssue.state == "open"? IssueState.open : IssueState.closed;
        }

        public Issue(BitBucketIssue bitBucketIssue)
        {
            Id = bitBucketIssue.id; 
            Title = bitBucketIssue.title;
            Description = bitBucketIssue.content.raw;
            State = bitBucketIssue.state == "new"? IssueState.open : IssueState.closed;
        }
        public int CompareTo(object? obj)
        {
            if (obj == null)
                return 1;
            Issue issue = obj as Issue;
            return this.Id.CompareTo(issue.Id);
        }
    }

    public enum IssueState
    {
        open,
        closed,    
    }
}
