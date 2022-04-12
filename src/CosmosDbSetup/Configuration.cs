namespace CosmosDbSetup;

public class Configuration
{
    public string DatabaseName { get; set; }
    public List<Container> Containers { get; set; }

    public Configuration()
    {
        DatabaseName = string.Empty;
        Containers = new List<Container>();
    }
}

