namespace WebCrawler;

using System.Net;
using System.Text.RegularExpressions;

public static class ContentProcessor
{
    public static ProcessedContent ProcessContent(PageContent content)
    {
        var processed = new ProcessedContent
        (
            content.Url,
            CleanText(content.Title),
            CleanText(content.MainContent),
            content.Headers.Select(CleanText).ToList(),
            content.Metadata.ToDictionary(kv => kv.Key, kv => CleanText(kv.Value)),
            content.ImageAltTexts.Select(CleanText).ToList(),
            content.Timestamp
        );

        return processed;
    }

    private static string CleanText(string text)
    {
        // Remove HTML tags
        text = Regex.Replace(text, "<.*?>", string.Empty);

        // Handle special characters
        text = WebUtility.HtmlDecode(text);

        // Normalize whitespace
        text = Regex.Replace(text, @"\s+", " ").Trim();

        return text;
    }
}
