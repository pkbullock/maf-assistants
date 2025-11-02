using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Sites.Item.Lists.Item.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Services
{
    /// <summary>
    /// Service for SharePoint operations via Microsoft Graph.
    /// </summary>
    public class SharePointService : GraphService
    {
        /// <summary>
        /// Gets files from a SharePoint site document library.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="driveId">The drive ID (document library).</param>
        /// <param name="maxItems">Maximum number of items to retrieve (optional, uses config default if not specified).</param>
        /// <returns>List of file names and their metadata.</returns>
        public async Task<List<DriveItem>> GetFilesAsync(string siteId, string driveId, int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var items = await GraphClient.Drives[driveId].Items["root"].Children
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = itemsToRetrieve;
                });

            return items?.Value?.Where(i => i.File != null).ToList() ?? new List<DriveItem>();
        }

        /// <summary>
        /// Gets the content of a file from SharePoint.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="driveId">The drive ID (document library).</param>
        /// <param name="itemId">The item ID of the file.</param>
        /// <returns>The file content as a string.</returns>
        public async Task<string> GetFileContentAsync(string siteId, string driveId, string itemId)
        {
            var stream = await GraphClient.Drives[driveId].Items[itemId].Content.GetAsync();
            if (stream == null)
                return string.Empty;

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Uploads or updates a file to SharePoint.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="driveId">The drive ID (document library).</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The content to write.</param>
        /// <returns>The created or updated DriveItem.</returns>
        public async Task<DriveItem?> UploadFileAsync(string siteId, string driveId, string fileName, string content)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var uploadedItem = await GraphClient.Drives[driveId].Items["root"]
                .ItemWithPath(fileName)
                .Content
                .PutAsync(stream);

            return uploadedItem;
        }

        /// <summary>
        /// Lists all sites accessible to the application.
        /// </summary>
        /// <param name="maxItems">Maximum number of sites to retrieve (optional, uses config default if not specified).</param>
        /// <returns>List of site objects.</returns>
        public async Task<List<Site>> GetSitesAsync(int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            var sites = await GraphClient.Sites.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = itemsToRetrieve;
            });

            return sites?.Value?.ToList() ?? new List<Site>();
        }

        /// <summary>
        /// Gets drives (document libraries) from a SharePoint site.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <returns>List of drives.</returns>
        public async Task<List<Drive>> GetDrivesAsync(string siteId)
        {
            var drives = await GraphClient.Sites[siteId].Drives.GetAsync();
            return drives?.Value?.ToList() ?? new List<Drive>();
        }

        /// <summary>
        /// Gets a summary of files from SharePoint that fits within context window constraints.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="driveId">The drive ID (document library).</param>
        /// <param name="maxItems">Maximum number of items to retrieve.</param>
        /// <returns>A formatted string summary of files.</returns>
        public async Task<string> GetFilesSummaryAsync(string siteId, string driveId, int? maxItems = null)
        {
            var files = await GetFilesAsync(siteId, driveId, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {files.Count} files:");

            foreach (var file in files)
            {
                summary.AppendLine($"- {file.Name} (Size: {file.Size} bytes, Modified: {file.LastModifiedDateTime})");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many files ({files.Count}) to display full summary. Showing first 10:");
                foreach (var file in files.Take(10))
                {
                    summary.AppendLine($"- {file.Name} (Size: {file.Size} bytes)");
                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Gets SharePoint lists from a site.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="maxItems">Maximum number of lists to retrieve (optional, uses config default if not specified).</param>
        /// <returns>List of SharePoint lists.</returns>
        public async Task<List<List>> GetListsAsync(string siteId, int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            var lists = await GraphClient.Sites[siteId].Lists.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = itemsToRetrieve;
            });

            return lists?.Value?.ToList() ?? new List<List>();
        }

        /// <summary>
        /// Gets items from a SharePoint list with optional filtering and limiting.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="listId">The list ID.</param>
        /// <param name="maxItems">Maximum number of items to retrieve (optional, uses config default if not specified).</param>
        /// <param name="filter">OData filter string (optional).</param>
        /// <param name="expand">Fields to expand (optional, e.g., "fields").</param>
        /// <returns>List of SharePoint list items.</returns>
        public async Task<List<ListItem>> GetListItemsAsync(string siteId, string listId, int? maxItems = null, string? filter = null, string? expand = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            var items = await GraphClient.Sites[siteId].Lists[listId].Items.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = itemsToRetrieve;
                requestConfiguration.QueryParameters.Expand = expand != null ? new[] { expand } : new[] { "fields" };
                if (!string.IsNullOrEmpty(filter))
                {
                    requestConfiguration.QueryParameters.Filter = filter;
                }
            });

            return items?.Value?.ToList() ?? new List<ListItem>();
        }

        /// <summary>
        /// Converts SharePoint list items to a markdown table format.
        /// </summary>
        /// <param name="items">The list items to convert.</param>
        /// <param name="fieldNames">Optional list of field names to include. If null, all fields are included.</param>
        /// <returns>A markdown-formatted table string.</returns>
        public string ConvertListItemsToMarkdownTable(List<ListItem> items, List<string>? fieldNames = null)
        {
            if (items == null || items.Count == 0)
                return "No items to display.";

            var markdown = new StringBuilder();
            
            // Get all unique field names from all items
            var allFields = new HashSet<string>();
            foreach (var item in items)
            {
                if (item.Fields?.AdditionalData != null)
                {
                    foreach (var key in item.Fields.AdditionalData.Keys)
                    {
                        // Skip internal SharePoint fields
                        if (!key.StartsWith("@") && !key.StartsWith("_"))
                        {
                            allFields.Add(key);
                        }
                    }
                }
            }

            // Use specified field names or all fields
            var columnsToShow = fieldNames != null && fieldNames.Count > 0 
                ? fieldNames.Where(f => allFields.Contains(f)).ToList()
                : allFields.OrderBy(f => f).ToList();

            if (columnsToShow.Count == 0)
                return "No displayable fields found.";

            // Create header row
            markdown.Append("| ");
            markdown.Append(string.Join(" | ", columnsToShow));
            markdown.AppendLine(" |");

            // Create separator row
            markdown.Append("| ");
            markdown.Append(string.Join(" | ", columnsToShow.Select(_ => "---")));
            markdown.AppendLine(" |");

            // Create data rows
            foreach (var item in items)
            {
                markdown.Append("| ");
                var values = new List<string>();
                foreach (var field in columnsToShow)
                {
                    var value = string.Empty;
                    if (item.Fields?.AdditionalData != null && 
                        item.Fields.AdditionalData.TryGetValue(field, out var fieldValue))
                    {
                        value = fieldValue?.ToString() ?? string.Empty;
                        // Escape pipe characters in values
                        value = value.Replace("|", "\\|");
                        // Replace newlines with spaces
                        value = value.Replace("\n", " ").Replace("\r", " ");
                    }
                    values.Add(value);
                }
                markdown.Append(string.Join(" | ", values));
                markdown.AppendLine(" |");
            }

            return markdown.ToString();
        }

        /// <summary>
        /// Gets a summary of SharePoint lists from a site.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="maxItems">Maximum number of lists to retrieve.</param>
        /// <returns>A formatted string summary of lists.</returns>
        public async Task<string> GetListsSummaryAsync(string siteId, int? maxItems = null)
        {
            var lists = await GetListsAsync(siteId, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {lists.Count} SharePoint lists:");

            foreach (var list in lists)
            {
                summary.AppendLine($"- {list.DisplayName} (ID: {list.Id})");
            }

            return summary.ToString();
        }
    }
}
