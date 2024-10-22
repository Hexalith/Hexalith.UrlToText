global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Threading.Tasks;

global using AngleSharp.Dom;

global using WebCrawler.Models;
using WebCrawler;

try
{
    // Load configuration
    var config = new ConfigurationManager("config.json");

    // Ensure storage directory exists
    DirectoryInfo outputDirectory = Directory.CreateDirectory(Path.Combine(config.OutputLocation, config.OutputName));

    // Initialize crawler
    var crawler = new Crawler(
        config.MaxDepth,
        10, // 10 concurrent requests
        outputDirectory.FullName,
        config.AcceptExternalLinks, // Add this line
        config.StartUrl // Add this line
    );

    Console.WriteLine("Starting web crawl...");
    await crawler.CrawlAsync(config.StartUrl);
    Console.WriteLine("Web crawl completed.");

    // Retrieve all stored content
    Console.WriteLine($"Retrieving stored content to '{outputDirectory.FullName}'...");
    var storageManager = new StorageManager(outputDirectory.FullName); // Use the output location from config

    IEnumerable<ProcessedContent> contents = await storageManager.RetrieveAllContentsAsync();
    Console.WriteLine($"Retrieved {contents.Count()} content items");

    // Generate output
    string outputFile = Path.Combine(config.OutputLocation, config.OutputName, $"{config.OutputName}.{config.OutputFormat}");
    if (config.OutputFormat.Equals("txt", StringComparison.OrdinalIgnoreCase))
    {
        await OutputGenerator.GenerateTextFileAsync(contents, outputFile);
    }
    else if (config.OutputFormat.Equals("pdf", StringComparison.OrdinalIgnoreCase))
    {
        await OutputGenerator.GeneratePdfAsync(contents, outputFile);
    }
    else
    {
        throw new ArgumentException("Unsupported output format specified in configuration.");
    }
    Console.WriteLine($"File '{outputFile}' generated.");

    Console.WriteLine("Web crawler application completed successfully.");
}
catch (Exception ex)
{
    ErrorHandler.LogError(ex, "Main application");
    Console.WriteLine($"An error occurred: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Console.WriteLine("Please check the log file for details.");
}
