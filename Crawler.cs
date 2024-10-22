namespace WebCrawler;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AngleSharp;

using WebCrawler.Models;

public class Crawler
{
    private readonly IBrowsingContext _context;
    private readonly HashSet<string> _visitedUrls;
    private readonly int _maxDepth;
    private readonly string _outputLocation;
    private readonly SemaphoreSlim _semaphore;
    private readonly StorageManager _storageManager;
    private readonly bool _acceptExternalLinks;
    private readonly Uri _initialUri;

    public Crawler(int maxDepth, int maxConcurrentRequests, string outputLocation, bool acceptExternalLinks, string initialUrl)
    {
        _context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        _visitedUrls = [];
        _maxDepth = maxDepth;
        _outputLocation = outputLocation;
        _semaphore = new SemaphoreSlim(maxConcurrentRequests);
        _storageManager = new StorageManager(outputLocation);
        _acceptExternalLinks = acceptExternalLinks;
        _initialUri = new Uri(initialUrl);
        Console.WriteLine($"Crawler initialized with max depth: {maxDepth}, max concurrent requests: {maxConcurrentRequests}, accept external links: {acceptExternalLinks}");
    }

    private void ClearOutputDirectory()
    {
        if (Directory.Exists(_outputLocation))
        {
            // Delete all files and subdirectories in the output directory
            foreach (string file in Directory.GetFiles(_outputLocation))
            {
                File.Delete(file);
            }
        }
    }

    public async Task CrawlAsync(string startUrl)
    {
        ClearOutputDirectory();
        Console.WriteLine($"Starting crawl from URL: {startUrl}");
        await CrawlRecursiveAsync(startUrl, 0);
        Console.WriteLine($"Crawl completed. Total URLs visited: {_visitedUrls.Count}");
    }

    private async Task CrawlRecursiveAsync(string url, int depth)
    {
        if (depth > _maxDepth || !_visitedUrls.Add(url))
        {
            return;
        }

        await _semaphore.WaitAsync();
        try
        {
            Console.WriteLine($"Crawling URL: {url} (Depth: {depth})");
            IDocument document = await _context.OpenAsync(url);
            Console.WriteLine($"Document loaded for URL: {url}");

            PageContent content = ContentExtractor.ExtractContentAsync(document);
            ProcessedContent processedContent = ContentProcessor.ProcessContent(content);

            var links = document.QuerySelectorAll("a")
                .Select(a => a.GetAttribute("href"))
                .Where(href => !string.IsNullOrEmpty(href))
                .Select(href => new Uri(new Uri(url), href).AbsoluteUri)
                .Where(href => IsValidLink(href, url, out bool isExternal))
                .Distinct()
                .ToList();

            var finalProcessedContent = new ProcessedContent(
                url,
                document.Title ?? string.Empty,
                processedContent.MainContent,
                links,
                processedContent.Metadata ?? new Dictionary<string, string>(),
                processedContent.ImageAltTexts ?? [],
                DateTime.UtcNow
            );

            await _storageManager.StoreContentAsync(finalProcessedContent);

            Console.WriteLine($"Content processed and stored for URL: {url}");
            Console.WriteLine($"Found {links.Count} valid links on page: {url}");

            foreach (Task? task in links.Select(link => CrawlRecursiveAsync(link, depth + 1)))
            {
                await task;
            }
            //IEnumerable<Task> tasks = links.Select(link => CrawlRecursiveAsync(link, depth + 1));
            //await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error crawling {url}: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"Finished processing URL: {url} (Depth: {depth})");
            _ = _semaphore.Release();
            Console.WriteLine($"Releasing semaphore for URL: {url} (Depth: {depth})");
        }
    }

    private bool IsValidLink(string href, string baseUrl, out bool isExternal)
    {
        isExternal = false;

        if (string.IsNullOrEmpty(href))
        {
            return false;
        }

        var baseUri = new Uri(baseUrl);

        if (Uri.TryCreate(baseUri, href, out Uri? hrefUri))
        {
            isExternal = hrefUri.Host != _initialUri.Host;

            if (_acceptExternalLinks)
            {
                return true;
            }
            else
            {
                return !isExternal;
            }
        }

        return false;
    }
}
