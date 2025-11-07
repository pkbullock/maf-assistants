# RSS Reader Service

This document describes the RSS Reader service added to the MAF Assistants framework.

## Overview

The RSS Reader service provides the ability to read and parse RSS/Atom feeds from websites, extracting structured information into easy-to-use objects. This enables AI agents and applications to consume news feeds, blog posts, and other syndicated content.

## Features

### 1. RSS/Atom Feed Parsing
- **Service**: `RssReaderService`
- Reads RSS 2.0 and Atom feeds from URLs
- Parses feed content using `System.ServiceModel.Syndication`
- Returns structured data as `RssItem` objects

### 2. Structured Data Extraction
- **Model**: `RssItem`
- **Extracted Information**:
  - **Title**: Article title
  - **Published Date**: When the article was published
  - **Content**: Full body/content of the article
  - **Excerpt**: Summary with HTML tags stripped (auto-generated if not available)
  - **Author**: Author name or email address
  - **Referenced URLs**: List of URLs found in the content
  - **Link**: Direct link to the full article

### 3. Content Processing
- **HTML Stripping**: Automatically removes HTML tags from excerpts for clean text
- **URL Extraction**: Identifies and extracts all URLs referenced in the content
- **Smart Excerpts**: Auto-generates 200-character excerpts when summary is not available
- **HTML Entity Decoding**: Converts HTML entities to readable text

## Configuration

No special configuration is required. The service works out of the box with any publicly accessible RSS or Atom feed URL.

### Dependencies

The service requires the following NuGet package (already included):
- `System.ServiceModel.Syndication` v8.0.0

## Usage Examples

The framework includes an RSS Reader service for reading and parsing RSS/Atom feeds from websites:

### Features
- **Feed Parsing**: Read RSS 2.0 and Atom feeds from any URL
- **Structured Data**: Extract title, published date, content, excerpt, author, and links
- **URL Extraction**: Automatically extract referenced URLs from content
- **HTML Processing**: Strip HTML tags and decode entities for clean text
- **Flexible Retrieval**: Read all items or limit to a specific number

### Quick Start
1. Create an instance of `RssReaderService`
2. Call `ReadFeedAsync` with the feed URL:
   ```csharp
   var rssService = new RssReaderService();
   var items = await rssService.ReadFeedAsync("https://example.com/feed.xml", maxItems: 10);
   
   foreach (var item in items)
   {
       Console.WriteLine($"{item.Title} - {item.Link}");
       Console.WriteLine($"Published: {item.PublishedDate}");
       Console.WriteLine($"Excerpt: {item.Excerpt}");
   }
   ```

### Basic Usage - Read All Items

```csharp
using MAF.Assistants.Services;

var rssService = new RssReaderService();
var items = await rssService.ReadFeedAsync("https://example.com/feed.xml");

foreach (var item in items)
{
    Console.WriteLine($"Title: {item.Title}");
    Console.WriteLine($"Published: {item.PublishedDate}");
    Console.WriteLine($"Author: {item.Author}");
    Console.WriteLine($"Link: {item.Link}");
    Console.WriteLine($"Excerpt: {item.Excerpt}");
    Console.WriteLine($"Content Length: {item.Content.Length} characters");
    Console.WriteLine($"Referenced URLs: {item.ReferencedUrls.Count}");
    Console.WriteLine();
}
```



### Read Limited Number of Items

```csharp
// Get only the 10 most recent items
var recentItems = await rssService.ReadFeedAsync("https://example.com/feed.xml", maxItems: 10);

foreach (var item in recentItems)
{
    Console.WriteLine($"{item.Title} - {item.Link}");
}
```

### Access Specific Properties

```csharp
var items = await rssService.ReadFeedAsync("https://blog.example.com/rss", maxItems: 5);

foreach (var item in items)
{
    // Title
    Console.WriteLine($"Article: {item.Title}");
    
    // Publication date
    if (item.PublishedDate.HasValue)
    {
        Console.WriteLine($"Published: {item.PublishedDate.Value:yyyy-MM-dd HH:mm}");
    }
    
    // Author information
    if (!string.IsNullOrEmpty(item.Author))
    {
        Console.WriteLine($"By: {item.Author}");
    }
    
    // Direct link
    Console.WriteLine($"Read more: {item.Link}");
    
    // Summary
    Console.WriteLine($"Summary: {item.Excerpt}");
    
    // Full content
    Console.WriteLine($"Content: {item.Content}");
    
    // Referenced URLs in the article
    if (item.ReferencedUrls.Any())
    {
        Console.WriteLine("Referenced URLs:");
        foreach (var url in item.ReferencedUrls)
        {
            Console.WriteLine($"  - {url}");
        }
    }
}
```

### Error Handling

```csharp
var rssService = new RssReaderService();

try 
{
    var items = await rssService.ReadFeedAsync("https://example.com/feed.xml");
    
    if (items.Count == 0)
    {
        Console.WriteLine("No items found in the feed.");
    }
}
catch (ArgumentException ex) 
{
    // Feed URL was null, empty, or whitespace
    Console.WriteLine($"Invalid URL: {ex.Message}");
}
catch (InvalidOperationException ex) 
{
    // Failed to read or parse the feed (network error, invalid XML, etc.)
    Console.WriteLine($"Failed to read feed: {ex.Message}");
}
```

## RssItem Properties

Each `RssItem` object contains the following properties:

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | The title of the RSS item |
| `PublishedDate` | `DateTimeOffset?` | When the item was published (nullable) |
| `Content` | `string` | The full content/body of the item |
| `Excerpt` | `string` | A summary with HTML stripped (max 200 chars if auto-generated) |
| `Author` | `string` | The author name or email address |
| `ReferencedUrls` | `List<string>` | URLs found in the content |
| `Link` | `string` | Direct link to the article |

## Integration with AI Agents

The RSS Reader service can be easily integrated into AI agents to provide them with access to news feeds and blog content.

### Example: Creating an AI Agent with RSS Reading Capability

```csharp
using MAF.Assistants.Services;
using Microsoft.Agents.AI;

// Create a wrapper function for the AI agent
public static async Task<string> ReadRssFeed(string feedUrl, int maxItems = 10)
{
    var rssService = new RssReaderService();
    var items = await rssService.ReadFeedAsync(feedUrl, maxItems);
    
    var summary = new StringBuilder();
    summary.AppendLine($"Found {items.Count} items in the RSS feed:\n");
    
    foreach (var item in items)
    {
        summary.AppendLine($"**{item.Title}**");
        summary.AppendLine($"Published: {item.PublishedDate}");
        summary.AppendLine($"Author: {item.Author}");
        summary.AppendLine($"Summary: {item.Excerpt}");
        summary.AppendLine($"Link: {item.Link}");
        summary.AppendLine();
    }
    
    return summary.ToString();
}

// Register with AI agent
AIAgent agent = client.CreateAIAgent(
    instructions: "You are a helpful assistant that can read RSS feeds.",
    name: "RssReaderAssistant",
    tools: [
        AIFunctionFactory.Create(ReadRssFeed, "read_rss_feed", "Reads and summarizes an RSS feed")
    ])
    .Build();
```

## Use Cases

### 1. News Aggregation
Monitor multiple news sources and aggregate the latest headlines:

```csharp
var newsSources = new[]
{
    "https://news.example.com/rss",
    "https://techblog.example.com/feed",
    "https://updates.example.org/rss.xml"
};

var rssService = new RssReaderService();
foreach (var source in newsSources)
{
    var items = await rssService.ReadFeedAsync(source, maxItems: 5);
    Console.WriteLine($"\n=== Latest from {source} ===");
    foreach (var item in items)
    {
        Console.WriteLine($"- {item.Title} ({item.PublishedDate})");
    }
}
```

### 2. Blog Monitoring
Track new posts from blogs and extract key information:

```csharp
var blogFeed = "https://blog.example.com/rss";
var rssService = new RssReaderService();
var posts = await rssService.ReadFeedAsync(blogFeed, maxItems: 3);

foreach (var post in posts)
{
    Console.WriteLine($"New Post: {post.Title}");
    Console.WriteLine($"Author: {post.Author}");
    Console.WriteLine($"Preview: {post.Excerpt}");
    Console.WriteLine($"Read full article: {post.Link}");
    
    // Extract all URLs mentioned in the post
    if (post.ReferencedUrls.Any())
    {
        Console.WriteLine("Referenced resources:");
        foreach (var url in post.ReferencedUrls)
        {
            Console.WriteLine($"  {url}");
        }
    }
}
```

### 3. Content Analysis
Analyze RSS feeds for specific topics or patterns:

```csharp
var rssService = new RssReaderService();
var items = await rssService.ReadFeedAsync("https://tech.example.com/feed.xml");

// Find articles about a specific topic
var aiArticles = items.Where(item => 
    item.Title.Contains("AI", StringComparison.OrdinalIgnoreCase) ||
    item.Content.Contains("artificial intelligence", StringComparison.OrdinalIgnoreCase))
    .ToList();

Console.WriteLine($"Found {aiArticles.Count} articles about AI:");
foreach (var article in aiArticles)
{
    Console.WriteLine($"- {article.Title}");
    Console.WriteLine($"  {article.Link}");
}
```

## Security Considerations

### SSRF (Server-Side Request Forgery)
- The service accepts arbitrary URLs and makes HTTP requests to them
- **Risk**: Could be exploited to scan internal networks or access internal services
- **Mitigation**: 
  - Implement URL validation or allowlisting for production environments with untrusted user input
  - Consider restricting to specific domains or URL patterns
  - Use a firewall or network policy to prevent access to internal resources

### Resource Consumption
- Very large RSS feeds could consume significant memory
- **Risk**: Potential denial of service through resource exhaustion
- **Mitigation**:
  - Always use the `maxItems` parameter to limit the number of items retrieved
  - Monitor memory usage in production environments
  - Consider implementing timeouts for HTTP requests
  - Set appropriate size limits on HTTP responses

### XML Security
- ✅ **XXE Protection**: The service uses .NET's `XmlReader` with default settings which provides protection against XML External Entity (XXE) attacks
- ✅ **Safe Parsing**: Uses the well-tested `System.ServiceModel.Syndication` library

### Best Practices for Production

```csharp
// Example: URL validation
public async Task<List<RssItem>> ReadFeedSafely(string feedUrl, int maxItems = 10)
{
    // Validate URL
    if (!Uri.TryCreate(feedUrl, UriKind.Absolute, out var uri))
    {
        throw new ArgumentException("Invalid URL format");
    }
    
    // Allowlist check (example)
    var allowedDomains = new[] { "example.com", "trusted-source.org" };
    if (!allowedDomains.Any(domain => uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase)))
    {
        throw new SecurityException($"Domain {uri.Host} is not in the allowlist");
    }
    
    // Proceed with reading the feed
    var rssService = new RssReaderService();
    return await rssService.ReadFeedAsync(feedUrl, maxItems);
}
```

## Implementation Details

### HTTP Client Usage
- The service uses a static `HttpClient` instance for HTTP requests
- This is acceptable for simple scenarios and avoids socket exhaustion
- For production applications with dependency injection, consider using `IHttpClientFactory`

### HTML Processing
- HTML tags are removed from excerpts using compiled regular expressions for performance
- HTML entities (e.g., `&amp;`, `&lt;`) are properly decoded
- Extra whitespace is normalized

### Excerpt Generation
- If the feed item has a summary, it's used as the excerpt (with HTML stripped)
- If no summary is available, an excerpt is generated from the content (first 200 characters)
- The excerpt length can be modified by changing the `DefaultExcerptLength` constant in the source

### URL Extraction
- Uses a compiled regex pattern to find URLs in content: `https?://[^\s<>"']+`
- Removes trailing punctuation (., , ) ;) that might be part of surrounding text
- Returns unique URLs only (duplicates removed)

## Troubleshooting

### Feed Not Loading
- **Error**: `InvalidOperationException: Failed to read RSS feed`
- **Solutions**:
  - Verify the URL is correct and publicly accessible
  - Check if the feed requires authentication (not currently supported)
  - Ensure the feed is valid RSS/Atom XML
  - Check network connectivity and firewall rules

### Empty Results
- **Issue**: `items.Count == 0`
- **Solutions**:
  - Verify the feed actually contains items
  - Some feeds may be empty if there are no recent posts
  - Check the feed URL in a browser or RSS reader

### Missing Content
- **Issue**: Some properties are empty or null
- **Solutions**:
  - Not all RSS feeds include all fields (author, content, etc.)
  - The service handles missing fields gracefully with empty strings
  - Check the actual feed XML to see what data is available

### HTML in Excerpts
- **Issue**: HTML tags still appear in excerpts
- **Solutions**:
  - The service should strip most HTML tags automatically
  - Complex HTML structures might leave some formatting
  - The `StripHtml` method is designed for basic HTML removal

## Architecture

```
Services/
  ├── RssReaderService.cs      # Main RSS reading service
  └── RssItem.cs               # Data model for RSS items
```

### Class Diagram

```
RssReaderService
├── ReadFeedAsync(string feedUrl) → List<RssItem>
├── ReadFeedAsync(string feedUrl, int maxItems) → List<RssItem>
└── [Private Helper Methods]
    ├── ConvertToRssItem(SyndicationItem item) → RssItem
    ├── GetAuthor(SyndicationItem item) → string
    ├── GetContent(SyndicationItem item) → string
    ├── GetExcerpt(SyndicationItem item, string content) → string
    ├── ExtractUrls(string content) → List<string>
    └── StripHtml(string html) → string

RssItem (Data Model)
├── Title: string
├── PublishedDate: DateTimeOffset?
├── Content: string
├── Excerpt: string
├── Author: string
├── ReferencedUrls: List<string>
└── Link: string
```

## Testing

The service includes comprehensive unit tests covering:
- Service instantiation
- Method signatures
- Input validation (null, empty, whitespace URLs)
- Property getters and setters
- Default values
- Method availability

Run tests:
```bash
dotnet test --filter "FullyQualifiedName~RssReaderService"
```

## Future Enhancements

Potential additions to consider:
- Support for authenticated feeds
- Caching of feed data to reduce network requests
- Feed validation and health checking
- Support for podcast feeds (enclosures)
- Feed discovery from website URLs
- Configurable excerpt length
- Support for media RSS extensions
- Timezone handling improvements
- Parallel feed fetching for multiple sources

## Related Documentation

- [Microsoft Graph Integration](GRAPH_INTEGRATION.md) - For integrating with Microsoft 365 services
- [System.ServiceModel.Syndication Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.servicemodel.syndication) - Official .NET documentation
