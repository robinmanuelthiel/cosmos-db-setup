using System.Text.Json;
using FluentValidation;

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

    public static Configuration GetConfigFromArgs(string[] args)
    {
        try
        {
            var json = args[0];
            Console.WriteLine($"Parsing configuration from JSON: {json}");
            var config = JsonSerializer.Deserialize<Configuration>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (config == null)
            {
                throw new Exception("Could not parse configuration.");
            }

            var validator = new ConfigurationValidator();
            validator.ValidateAndThrow(config);
            return config;
        }
        catch (IndexOutOfRangeException)
        {
            throw new Exception("Missing required argument. Please provide a configuration via args");
        }
        catch (Exception ex)
        {
            throw new Exception("Error parsing arguments.", ex);
        }
    }
}
