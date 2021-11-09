namespace Host;

public static class ConfigurationExtentions
{
    public static string GetValueOrDefault(this IConfiguration configuration, string key, string defaultValue)
    {
        var value = configuration.GetValue<string>(key);
        return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }
}
