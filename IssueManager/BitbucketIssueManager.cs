using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace IssueManager
{
    public class BitbucketIssueManager : IIssueManager
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiBaseUrl;

        public BitbucketIssueManager(string ownerName, string repositoryName, string authToken)
        {
            ApiBaseUrl = $"https://api.bitbucket.org/2.0/repositories/{ownerName}/{repositoryName}/issues";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task AddIssueAsync(Issue issue)
        {
            var newIssue = new BitBucketIssue { title = issue.Title, content = new Content {raw = issue.Description }, state = issue.State == IssueState.open ? "new" : "resolved" };
            var json = JsonSerializer.Serialize(newIssue);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiBaseUrl, content);
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

        public async Task UpdateIssueAsync(int issueNumber, Issue issue)
        {
            var dataToModify = new BitBucketIssue {id = issueNumber, title = issue.Title, content = new Content { raw = issue.Description}, state = issue.State == IssueState.open ? "new" : "resolved" };
            var json = JsonSerializer.Serialize(dataToModify);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{ApiBaseUrl}/{issueNumber}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Issue>> GetIssuesAsync()
        {
            var response = await _httpClient.GetAsync(ApiBaseUrl);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            JsonNode jsonNode = JsonNode.Parse(responseBody);
            JsonNode value = jsonNode!["values"];
            JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
            var deserializeJson = value.Deserialize<List<BitBucketIssue>>();
            List<Issue> Issues = new List<Issue>();
            deserializeJson.ForEach(x => Issues.Add(new Issue(x)));
            return Issues;  
        }
    }
}
