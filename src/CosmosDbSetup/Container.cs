namespace CosmosDbSetup;

public class Container
{
    public string Name { get; set; }
    public string PartitionKey { get; set; }

    public Container()
    {
        Name = string.Empty;
        PartitionKey = string.Empty;
    }
}
