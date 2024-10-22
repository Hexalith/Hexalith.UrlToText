namespace WebCrawler;

using System.Text;

public static class OutputGenerator
{
    public static async Task GenerateTextFileAsync(IEnumerable<ProcessedContent> contents, string outputPath)
    {
        var sb = new StringBuilder();

        foreach (ProcessedContent content in contents)
        {
            _ = sb.AppendLine($"URL: {content.Url}");
            _ = sb.AppendLine($"Title: {content.Title}");
            _ = sb.AppendLine($"Content: {content.MainContent}");
            _ = sb.AppendLine(new string('-', 80));
        }

        await File.WriteAllTextAsync(outputPath, sb.ToString());
    }

    public static Task GeneratePdfAsync(IEnumerable<ProcessedContent> contents, string outputPath) =>
        // Implementation using a PDF library like iTextSharp or PdfSharp
        throw new NotImplementedException("PDF generation is not implemented yet.");
}
