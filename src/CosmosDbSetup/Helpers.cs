namespace CosmosDbSetup;

public static class Helpers
{
    public static string GetFromEnv(string name, string? defaultValue = null)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            if (defaultValue == null)
            {
                throw new Exception($"Environment variable '{name}' is not set.");
            }
            else
            {
                return defaultValue;
            }
        }
        return value;
    }
}
