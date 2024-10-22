namespace WebCrawler;

using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;

[DataContract]
public class ConfigurationManager
{
    [DataMember]
    public int MaxDepth { get; set; }
    [DataMember]
    public int MaxConcurrentRequests { get; set; }
    [DataMember]
    public bool AcceptExternalLinks { get; set; }
    [DataMember]
    public string OutputFormat { get; set; }
    [DataMember]
    public string OutputName { get; set; }
    [DataMember]
    public string OutputLocation { get; set; }
    [DataMember]
    public string StartUrl { get; set; }

    public ConfigurationManager()
        => OutputName = OutputFormat = OutputLocation = StartUrl = string.Empty;

    public ConfigurationManager(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        string jsonString = File.ReadAllText(configPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        ConfigurationManager? config = JsonSerializer.Deserialize<ConfigurationManager>(jsonString, options);

        if (config == null)
        {
            throw new InvalidOperationException("Failed to deserialize configuration file.");
        }

        MaxDepth = config.MaxDepth;
        MaxConcurrentRequests = config.MaxConcurrentRequests;
        OutputFormat = config.OutputFormat;
        OutputLocation = config.OutputLocation;
        StartUrl = config.StartUrl;
        AcceptExternalLinks = config.AcceptExternalLinks;
        OutputName = config.OutputName;
    }
}
