namespace WebCrawler;

using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class StorageManager(string StoragePath)
{
    public async Task StoreContentAsync(ProcessedContent content)
    {
        string fileName = $"{content.Url.GetHashCode()}.json";
        string filePath = Path.Combine(StoragePath, fileName);

        string json = JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<ProcessedContent?> RetrieveContentAsync(string url)
    {
        string fileName = $"{url.GetHashCode()}.json";
        string filePath = Path.Combine(StoragePath, fileName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ProcessedContent>(json);
    }

    public async Task<IEnumerable<ProcessedContent>> RetrieveAllContentsAsync()
    {
        var contents = new List<ProcessedContent>();
        foreach (string file in Directory.GetFiles(StoragePath, "*.json"))
        {
            string json = await File.ReadAllTextAsync(file);
            ProcessedContent? content = JsonSerializer.Deserialize<ProcessedContent>(json);
            if (content != null)
            {
                contents.Add(content);
            }
        }
        return contents;
    }
}
