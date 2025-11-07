using System;
using System.Collections.Generic;

namespace MAF.Assistants.Models
{
    /// <summary>
    /// Represents an RSS feed item with key information.
    /// </summary>
    public class RssItem
    {
        /// <summary>
        /// Gets or sets the title of the RSS item.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the published date of the RSS item.
        /// </summary>
        public DateTimeOffset? PublishedDate { get; set; }

        /// <summary>
        /// Gets or sets the full content/body of the RSS item.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the excerpt/summary of the RSS item.
        /// </summary>
        public string Excerpt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the author of the RSS item.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of URLs referenced in the RSS item.
        /// </summary>
        public List<string> ReferencedUrls { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the link to the article.
        /// </summary>
        public string Link { get; set; } = string.Empty;
    }
}
