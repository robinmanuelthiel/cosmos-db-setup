using CosmosDbSetup;
using Microsoft.Azure.Cosmos;
using Polly;

var config = Helpers.GetConfigFromArgs(args);
string connectionString = Helpers.GetFromEnv("CONNECTION_STRING", "AccountEndpoint=https://cosmosdb:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

CosmosClient client = new CosmosClient(connectionString, new CosmosClientOptions()
{
    HttpClientFactory = () =>
    {
        HttpMessageHandler httpMessageHandler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
        };
        return new HttpClient(httpMessageHandler);
    },
    ConnectionMode = ConnectionMode.Gateway
});

await Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
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
