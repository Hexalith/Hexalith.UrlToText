namespace WebCrawler;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;

public static class ContentExtractor
{
    public static PageContent ExtractContentAsync(IDocument document)
    {
        var content = new PageContent
        (
            document.Url?.ToString() ?? string.Empty,
            document.Title ?? string.Empty,
            ExtractMainContent(document),
            ExtractHeaders(document),
            ExtractMetadata(document),
            ExtractImageAltTexts(document),
            DateTime.UtcNow
        );

        return content;
    }


    private static string ExtractMainContent(IDocument document)
    {
        // Implémentation simple pour extraire le contenu principal
        IHtmlElement? body = document.Body;
        return body?.TextContent ?? string.Empty;
    }

    private static List<string> ExtractHeaders(IDocument document) =>
        // Extraire tous les en-têtes h1 à h6
        document.QuerySelectorAll("h1, h2, h3, h4, h5, h6")
            .Select(h => h.TextContent)
            .ToList();

    private static Dictionary<string, string> ExtractMetadata(IDocument document) =>
        // Extraire les métadonnées des balises meta
        document.QuerySelectorAll("meta")
            .Where(m => m.HasAttribute("name") && m.HasAttribute("content"))
            .ToDictionary(
                m => m.GetAttribute("name") ?? string.Empty,
                m => m.GetAttribute("content") ?? string.Empty
            );

    private static List<string> ExtractImageAltTexts(IDocument document) =>
        // Extraire les textes alternatifs des images
        document.QuerySelectorAll("img")
            .Select(img => img.GetAttribute("alt") ?? string.Empty)
            .Where(alt => !string.IsNullOrWhiteSpace(alt))
            .ToList();
}
