using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IssueManager
{
    public class GitHubIssueManager: IIssueManager
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiBaseUrl;

        public GitHubIssueManager(string owner, string repository, string authToken)
        {

            ApiBaseUrl = $"https://api.github.com/repos/{owner}/{repository}/issues";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHubIssueManager");
        }

        public async Task<List<Issue>> GetIssuesAsync()
        {
            var response = await _httpClient.GetAsync(ApiBaseUrl);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            List<Issue> IssueList = new List<Issue>();
            JsonSerializer.Deserialize<List<GitHubIssue>>(responseBody).ForEach(x => { IssueList.Add(new Issue(x)); });
            return IssueList;
        }

        public async Task AddIssueAsync(Issue issue)
        {
            var newIssue = new GitHubIssue { title = issue.Title, body = issue.Description, state = issue.State == IssueState.open?"open":"closed" };
            var json = JsonSerializer.Serialize(newIssue);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiBaseUrl, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateIssueAsync(int issueNumber, Issue issue)
        {
            var dataToModify= new GitHubIssue {number = issueNumber, title = issue.Title, body = issue.Description, state = issue.State == IssueState.open ? "open" : "closed" };
            var json = JsonSerializer.Serialize(dataToModify);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{ApiBaseUrl}/{issueNumber}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task CloseIssueAsync(int issueNumber, Issue issue)
        {
            issue.State = IssueState.closed;
            await UpdateIssueAsync(issueNumber, issue);
        }

        public async Task ExportIssuesToFileAsync(string filePath)
        {
            var issues = await GetIssuesAsync();
            var json = JsonSerializer.Serialize(issues);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task ImportIssuesFromFileAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var importedIssues = JsonSerializer.Deserialize<List<Issue>>(json);
            var currentIssues = await GetIssuesAsync();
            var onlyNewIssues = importedIssues.Except(currentIssues, new IssueComparer()).ToList();
            foreach (var issue in onlyNewIssues)
            {
                await AddIssueAsync(issue);
            }
        }
    }
}