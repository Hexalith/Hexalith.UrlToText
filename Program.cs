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
    Console.WriteLine("Web crawler application starting...");

    // Load configuration
    var config = new ConfigurationManager("config.json");
    Console.WriteLine($"Configuration loaded. Max depth: {config.MaxDepth}, Output format: {config.OutputFormat}");

    // Ensure storage directory exists
    Directory.CreateDirectory(config.OutputLocation);
    Console.WriteLine($"Storage directory created/verified: {config.OutputLocation}");

    // Initialize crawler
    var crawler = new Crawler(
        config.MaxDepth,
        10, // 10 concurrent requests
        config.OutputLocation,
        config.AcceptExternalLinks, // Add this line
        config.StartUrl // Add this line
    );
    Console.WriteLine("Crawler initialized");

    Console.WriteLine("Starting web crawl...");
    await crawler.CrawlAsync(config.StartUrl);
    Console.WriteLine("Web crawl completed.");

    Console.WriteLine("About to retrieve stored content...");
    
    // Retrieve all stored content
    Console.WriteLine("Retrieving stored content...");
    var storageManager = new StorageManager(config.OutputLocation); // Use the output location from config
    Console.WriteLine($"StorageManager instance created with path: {config.OutputLocation}");
    
    Console.WriteLine("Calling RetrieveAllContentsAsync...");
    var contents = await storageManager.RetrieveAllContentsAsync();
    Console.WriteLine($"Retrieved {contents.Count()} content items");

    // Generate output
    Console.WriteLine($"Generating output in {config.OutputFormat} format...");
    if (config.OutputFormat.Equals("txt", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Generating text file...");
        await OutputGenerator.GenerateTextFileAsync(contents, config.OutputLocation);
        Console.WriteLine("Text file generated.");
    }
    else if (config.OutputFormat.Equals("pdf", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Generating PDF file...");
        await OutputGenerator.GeneratePdfAsync(contents, config.OutputLocation);
        Console.WriteLine("PDF file generated.");
    }
    else
    {
        throw new ArgumentException("Unsupported output format specified in configuration.");
    }

    Console.WriteLine($"Output generated at: {config.OutputLocation}");
    Console.WriteLine("Web crawler application completed successfully.");
}
catch (Exception ex)
{
    ErrorHandler.LogError(ex, "Main application");
    Console.WriteLine($"An error occurred: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Console.WriteLine("Please check the log file for details.");
}
