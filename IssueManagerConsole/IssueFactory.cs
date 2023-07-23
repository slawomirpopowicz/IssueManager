using IssueManager;

namespace IssueManagerConsole
{

    internal class IssueFactory
    {
        const string gitHubOwnerName = "your-repository-owner";//"your-repository-owner";
        const string gitHubRepositoryName = "your-repository-name";//"your-repository-name";
        const string gitHubAuthToken = "your-personal-access-token";//"your-personal-access-token";

        
        const string bitbucketRepositoryName = "your-repository-owner";//"your-repository-owner";
        const string bitbucketOwnerName = "your-repository-name";//"your-repository-name";
        const string bitbuckerAuthToken = "your-personal-access-token";//"your-personal-access-token";


        private readonly string _hostingServiceName;
        private readonly IIssueManager _issueManager;
        public IssueFactory(string option)
        {
            switch (option)
            {
                case "1":
                    _issueManager = new GitHubIssueManager(gitHubOwnerName, gitHubRepositoryName, gitHubAuthToken);
                    _hostingServiceName = "GitHub";
                    break;
                case "2":
                    _issueManager = new BitbucketIssueManager(bitbucketOwnerName, bitbucketRepositoryName, bitbuckerAuthToken);
                    _hostingServiceName = "BitBucket";
                    break;
            }
        }

        public async Task Run()
        {
            while (true)
            {
                Console.WriteLine($"Welcome to the {_hostingServiceName} Issue Manager!");

                Console.WriteLine("  Options:");
                Console.WriteLine("  1. List all issues");
                Console.WriteLine("  2. Add a new issue");
                Console.WriteLine("  3. Modify an issue");
                Console.WriteLine("  4. Close an issue");
                Console.WriteLine("  5. Export issues to a file");
                Console.WriteLine("  6. Import issues from a file");
                Console.WriteLine("  7. Exit");

                Console.Write("Select an option (1-7): ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ListIssues(_issueManager);
                        break;
                    case "2":
                        await AddNewIssue(_issueManager);
                        break;
                    case "3":
                        await ModifyIssue(_issueManager);
                        break;
                    case "4":
                        await CloseIssue(_issueManager);
                        break;
                    case "5":
                        await ExportIssues(_issueManager);
                        break;
                    case "6":
                        await ImportIssues(_issueManager);
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private async Task ListIssues(IIssueManager manager)
        {
            Console.Clear();
            var issues = await manager.GetIssuesAsync();

            if (issues.Count > 0)
            {
                Console.WriteLine("\nIssues:");
                issues.Sort();
                foreach (var issue in issues)
                {
                    Console.WriteLine($"#{issue.Id}: {issue.Title}, Description: {issue.Description}, State: {(issue.State == IssueState.open ? "Open" : "Closed")}");
                }
            }
            else
            {
                Console.WriteLine("\nNo issues found.");
            }
        }

        private async Task AddNewIssue(IIssueManager manager)
        {
            Console.Clear();
            Console.Write("Enter issue name: ");
            var name = Console.ReadLine();

            Console.Write("Enter issue description: ");
            var description = Console.ReadLine();

            var issue = new Issue {Title = name, Description = description };
            await manager.AddIssueAsync(issue);
            Console.WriteLine("Issue added successfully.");
        }

        private async Task ModifyIssue(IIssueManager manager)
        {
            Console.Clear();
            Console.Write("Enter the issue number to modify: ");
            var issueNumberStr = Console.ReadLine();

            if (int.TryParse(issueNumberStr, out int issueNumber))
            {
                var issues = await manager.GetIssuesAsync();
                var issueToModify = issues.Single(issue => issue.Id == issueNumber);
                if (issueToModify != null)
                {
                    Console.Write("Enter new issue name: ");
                    issueToModify.Title = Console.ReadLine();

                    Console.Write("Enter new issue description: ");
                    issueToModify.Description = Console.ReadLine();

                    await manager.UpdateIssueAsync(issueNumber, issueToModify);
                    Console.WriteLine("Issue modified successfully.");
                }
                else
                {
                    Console.WriteLine("Issue not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid issue number.");
            }
        }

        private async Task CloseIssue(IIssueManager manager)
        {
            Console.Clear();
            Console.Write("Enter the issue number to close: ");
            var issueNumberStr = Console.ReadLine();

            if (int.TryParse(issueNumberStr, out int issueNumber))
            {
                var issues = await manager.GetIssuesAsync();
                var issueToClose = issues.Single(issue => issue.Id == issueNumber);
                if (issueToClose != null)
                {
                    await manager.CloseIssueAsync(issueNumber, issueToClose);
                    Console.WriteLine("Issue closed successfully.");
                }
                else
                {
                    Console.WriteLine("Issue not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid issue number.");
            }
        }

        private async Task ExportIssues(IIssueManager manager)
        {
            Console.Clear();
            Console.Write("Enter the file path to export issues: ");
            var filePath = Console.ReadLine();

            await manager.ExportIssuesToFileAsync(filePath);
            Console.WriteLine("Issues exported to file successfully.");
        }

        private async Task ImportIssues(IIssueManager manager)
        {
            Console.Clear();
            Console.Write("Enter the file path to import issues: ");
            var filePath = Console.ReadLine();

            await manager.ImportIssuesFromFileAsync(filePath);
            Console.WriteLine("Issues imported from file successfully.");
        }
    }
}
