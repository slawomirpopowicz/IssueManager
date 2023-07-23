///<Summary>
///The application consists of two parts: the "IssueManager" library and the "IssueManagerConsole" console application. The "IIssueManager" interface has been implemented to support new hosting service implementations and is passed via Dependency Injection to the "IssueFactory" class. The "BitBucketIssue" and "GitHubIssue" classes handle JSON object serialization and deserialization, and they are converted into Issue class objects using copy constructors, which facilitate data manipulation. The "BitbucketIssueManager" and "GitHubIssueManager" classes perform operations based on the HTTP protocol, fulfilling the requirements from the task. However, the application may be further developed to include other useful functionalities, such as error logging. Additionally, some elements can be simplified by eliminating repeated methods in the "BitbucketIssueManager" and "GitHubIssueManager" classes, following the DRY (Don't Repeat Yourself) principle. 
///</Summary>


namespace IssueManagerConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IssueFactory factory = null;
            while (true)
            {
                Console.WriteLine("Choose which service you want to us\n");

                Console.WriteLine("Options:");
                Console.WriteLine("1. GitHub");
                Console.WriteLine("2. Bitucket");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option (1-3): ");
                string option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        factory = new IssueFactory(option);
                        break;
                    case "2":
                        factory = new IssueFactory(option);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key and try again.");
                        Console.ReadKey();
                        break;
                }
                Console.Clear();
                if (factory != null)
                {
                    try
                    {
                        await factory.Run();
                    }
                    catch(Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                }
            }            
        }
    }
}