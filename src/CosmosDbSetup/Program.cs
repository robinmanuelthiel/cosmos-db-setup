using CosmosDbSetup;
using Microsoft.Azure.Cosmos;
using Polly;

string connectionString = Helpers.GetFromEnv("CONNECTION_STRING", "AccountEndpoint=https://cosmosdb:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
string databaseName = Helpers.GetFromEnv("DATABASE_NAME", "database");
string containerName = Helpers.GetFromEnv("CONTAINER_NAME", "container");
string containerPartitionKey = Helpers.GetFromEnv("CONTAINER_PARTITION_KEY", "/id");

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
        await client.CreateDatabaseIfNotExistsAsync(databaseName);
        var database = client.GetDatabase(databaseName);
        await database.CreateContainerIfNotExistsAsync(containerName, containerPartitionKey);
    });

Console.WriteLine($"Done.");
