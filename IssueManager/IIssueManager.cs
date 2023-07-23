namespace IssueManager
{    public interface IIssueManager
    {
        Task<List<Issue>> GetIssuesAsync();
        Task AddIssueAsync(Issue issue);
        Task UpdateIssueAsync(int issueNumber, Issue issue);
        Task CloseIssueAsync(int issueNumber, Issue issue);
        Task ExportIssuesToFileAsync(string filePath);
        Task ImportIssuesFromFileAsync(string filePath);
    }
}
