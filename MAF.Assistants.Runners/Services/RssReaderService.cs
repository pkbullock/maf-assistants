using MAF.Assistants.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MAF.Assistants.Services
{
    /// <summary>
    /// Service for reading and parsing RSS feeds from websites.
    /// </summary>
    public class RssReaderService
    {
        // Note: Using static HttpClient is acceptable for this simple scenario.
        // For production applications with DI, consider using IHttpClientFactory.
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly Regex _urlRegex = new Regex(
            @"https?://[^\s<>""']+", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _htmlTagRegex = new Regex(
            @"<[^>]+>",
            RegexOptions.Compiled);
        private const int DefaultExcerptLength = 200;

        /// <summary>
        /// Reads an RSS feed from the specified URL.
        /// </summary>
        /// <param name="feedUrl">The URL of the RSS feed to read.</param>
        /// <returns>A list of RSS items from the feed.</returns>
        public async Task<List<RssItem>> ReadFeedAsync(string feedUrl)
        {
            if (string.IsNullOrWhiteSpace(feedUrl))
            {
                throw new ArgumentException("Feed URL cannot be null or empty.", nameof(feedUrl));
            }

            var rssItems = new List<RssItem>();

            try
            {
                using var response = await _httpClient.GetAsync(feedUrl);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var xmlReader = XmlReader.Create(stream);

                var feed = SyndicationFeed.Load(xmlReader);

                foreach (var item in feed.Items)
                {
                    rssItems.Add(ConvertToRssItem(item));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to read RSS feed from {feedUrl}: {ex.Message}", ex);
            }

            return rssItems;
        }

        /// <summary>
        /// Reads an RSS feed and returns a limited number of items.
        /// </summary>
        /// <param name="feedUrl">The URL of the RSS feed to read.</param>
        /// <param name="maxItems">Maximum number of items to retrieve.</param>
        /// <returns>A list of RSS items from the feed.</returns>
        public async Task<List<RssItem>> ReadFeedAsync(string feedUrl, int maxItems)
        {
            var allItems = await ReadFeedAsync(feedUrl);
            return allItems.Take(maxItems).ToList();
        }

        /// <summary>
        /// Converts a SyndicationItem to an RssItem.
        /// </summary>
        private RssItem ConvertToRssItem(SyndicationItem item)
        {
            var rssItem = new RssItem
            {
                Title = item.Title?.Text ?? string.Empty,
                PublishedDate = item.PublishDate,
                Link = item.Links?.FirstOrDefault()?.Uri?.ToString() ?? string.Empty,
                Author = GetAuthor(item)
            };

            // Extract content and excerpt
            var content = GetContent(item);
            rssItem.Content = content;
            rssItem.Excerpt = GetExcerpt(item, content);

            // Extract referenced URLs from content
            rssItem.ReferencedUrls = ExtractUrls(content);

            return rssItem;
        }

        /// <summary>
        /// Gets the author of the syndication item.
        /// </summary>
        private string GetAuthor(SyndicationItem item)
        {
            if (item.Authors?.Count > 0)
            {
                var author = item.Authors.First();
                return !string.IsNullOrWhiteSpace(author.Name) ? author.Name : author.Email ?? string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the content from the syndication item.
        /// </summary>
        private string GetContent(SyndicationItem item)
        {
            // Try to get content from Content property
            if (item.Content is TextSyndicationContent textContent)
            {
                return textContent.Text ?? string.Empty;
            }

            // Fallback to summary if content is not available
            if (item.Summary?.Text != null)
            {
                return item.Summary.Text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the excerpt from the syndication item.
        /// </summary>
        private string GetExcerpt(SyndicationItem item, string content)
        {
            // Try summary first
            if (item.Summary?.Text != null && !string.IsNullOrWhiteSpace(item.Summary.Text))
            {
                return StripHtml(item.Summary.Text);
            }

            // If no summary, create excerpt from content
            if (!string.IsNullOrWhiteSpace(content))
            {
                var plainText = StripHtml(content);
                return plainText.Length > DefaultExcerptLength 
                    ? plainText.Substring(0, DefaultExcerptLength) + "..." 
                    : plainText;
            }

            return string.Empty;
        }

        /// <summary>
        /// Extracts URLs from the content.
        /// </summary>
        private List<string> ExtractUrls(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new List<string>();
            }

            var matches = _urlRegex.Matches(content);
            return matches
                .Select(m => m.Value.TrimEnd('.', ',', ')', ';'))
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Strips HTML tags from a string.
        /// </summary>
        private string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            // Remove HTML tags
            var stripped = _htmlTagRegex.Replace(html, string.Empty);
            
            // Decode HTML entities
            stripped = System.Net.WebUtility.HtmlDecode(stripped);
            
            // Remove extra whitespace
            stripped = Regex.Replace(stripped, @"\s+", " ").Trim();

            return stripped;
        }
    }
}
