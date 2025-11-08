using Markdig;

namespace MAF.UI.Services;

public class MarkdownService
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownService()
    {
        // Configure Markdig pipeline with extensions
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions() // Includes tables, task lists, etc.
            .UseEmojiAndSmiley()
            .UseAutoLinks()
            .Build();
    }

    public string ConvertToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        try
        {
            var html = Markdown.ToHtml(markdown, _pipeline);
            return html;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting markdown: {ex.Message}");
            return $"<p>{System.Net.WebUtility.HtmlEncode(markdown)}</p>";
        }
    }
}
