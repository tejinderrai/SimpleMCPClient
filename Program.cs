using ConsoleMCPClient;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using System.Text.Json;
using System.Text.Json.Nodes;

// IMPORTANT!! - https://www.nuget.org/packages/ModelContextProtocol/absoluteLatest
class Program
{
    static async Task Main()
    {
        // Build configuration  
        var builder = new ConfigurationBuilder()
            .AddConfiguration(new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build());
            

        IConfiguration config = builder.Build(); // Create the configuration root  

        References.McpServerURI = config["MCPServerURL"] ?? "Not loaded";

        var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(References.McpServerURI),

        });

        Console.WriteLine("Enter your Microsoft Docs query:");
        string msDocsQuery = Console.ReadLine() ?? "no input";
        
        var parameters = new Dictionary<string, object?>
        {
            { "query", msDocsQuery }
        };

        var client = await McpClient.CreateAsync(clientTransport, new McpClientOptions(), null, new CancellationToken());

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var mcpServerResults = await client.CallToolAsync("microsoft_docs_search", parameters);
        
        JsonNode mcpResponse = mcpServerResults.StructuredContent;
       
        JsonArray resultsArray = (mcpResponse)["results"] as JsonArray ?? new JsonArray();

        if (resultsArray != null)
        {
            foreach (var result in resultsArray)
            {
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine("Title: " + result["title"]);
                //Console.WriteLine("Content: " + result["content"]);
                Console.WriteLine("ContentUrl: " + result["contentUrl"]);
            }
        }
    }
}
