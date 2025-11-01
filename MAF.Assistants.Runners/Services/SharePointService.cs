using Microsoft.Graph;
using Microsoft.Graph.Models;
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
    }
}
