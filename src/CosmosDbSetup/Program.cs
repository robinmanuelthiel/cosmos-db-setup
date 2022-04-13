using CosmosDbSetup;
using Microsoft.Azure.Cosmos;
using Polly;

var delay = Convert.ToInt32(Helpers.GetFromEnv("DELAY", "0"));
if (delay > 0)
{
    Console.WriteLine($"Delaying for {delay} seconds");
    System.Threading.Thread.Sleep(delay * 1000);
}

var config = Helpers.GetConfigFromArgs(args);
string connectionString = Helpers.GetFromEnv("CONNECTION_STRING", "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
Console.WriteLine($"Connection string: {connectionString}");

CosmosClient client = new CosmosClient(connectionString, new CosmosClientOptions()
{
    HttpClientFactory = () =>
    {
        HttpMessageHandler httpMessageHandler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        return new HttpClient(httpMessageHandler);
    },
    ConnectionMode = ConnectionMode.Gateway
});

await Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
    {
        Console.WriteLine($"Retry {retryCount}/5...");
    })
    .ExecuteAsync(async () =>
    {
        Console.WriteLine($"Creating database {config.DatabaseName}...");
        await client.CreateDatabaseIfNotExistsAsync(config.DatabaseName);
        var database = client.GetDatabase(config.DatabaseName);
        Console.WriteLine($"Database {config.DatabaseName} created.");

        foreach (var container in config.Containers)
        {
            Console.WriteLine($"Creating container {container.Name}...");
            await database.CreateContainerIfNotExistsAsync(container.Name, container.PartitionKey);
            Console.WriteLine($"Container {container.Name} created.");
        }
    });

Console.WriteLine($"Done.");
