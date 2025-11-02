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
    /// Service for OneDrive operations via Microsoft Graph.
    /// </summary>
    public class OneDriveService : GraphService
    {
        /// <summary>
        /// Gets files from the user's OneDrive root folder.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="maxItems">Maximum number of items to retrieve (optional, uses config default if not specified).</param>
        /// <returns>List of drive items.</returns>
        public async Task<List<DriveItem>> GetFilesAsync(string userId = "me", int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            // First get the drive to get its ID
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
                return new List<DriveItem>();

            var items = await GraphClient.Drives[drive.Id].Items["root"].Children
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = itemsToRetrieve;
                });

            return items?.Value?.ToList() ?? new List<DriveItem>();
        }

        /// <summary>
        /// Gets files from a specific folder in OneDrive.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="folderId">The folder ID.</param>
        /// <param name="maxItems">Maximum number of items to retrieve.</param>
        /// <returns>List of drive items.</returns>
        public async Task<List<DriveItem>> GetFilesInFolderAsync(string userId, string folderId, int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
                return new List<DriveItem>();

            var items = await GraphClient.Drives[drive.Id].Items[folderId].Children
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = itemsToRetrieve;
                });

            return items?.Value?.ToList() ?? new List<DriveItem>();
        }

        /// <summary>
        /// Gets the content of a file from OneDrive.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="itemId">The item ID of the file.</param>
        /// <returns>The file content as a string.</returns>
        public async Task<string> GetFileContentAsync(string userId, string itemId)
        {
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
                return string.Empty;

            var stream = await GraphClient.Drives[drive.Id].Items[itemId].Content.GetAsync();
            if (stream == null)
                return string.Empty;

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Downloads file content as a byte array (for binary files like images).
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="itemId">The item ID of the file.</param>
        /// <returns>The file content as a byte array.</returns>
        public async Task<byte[]?> GetFileContentAsBytesAsync(string userId, string itemId)
        {
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
                return null;

            var stream = await GraphClient.Drives[drive.Id].Items[itemId].Content.GetAsync();
            if (stream == null)
                return null;

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Uploads or updates a file to OneDrive.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The content to write.</param>
        /// <returns>The created or updated DriveItem.</returns>
        public async Task<DriveItem?> UploadFileAsync(string userId, string fileName, string content)
        {
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
                return null;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var uploadedItem = await GraphClient.Drives[drive.Id].Items["root"]
                .ItemWithPath(fileName)
                .Content
                .PutAsync(stream);

            return uploadedItem;
        }

        /// <summary>
        /// Uploads or updates a binary file (like an image) to OneDrive.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The binary content to write.</param>
        /// <returns>The created or updated DriveItem.</returns>
        public async Task<DriveItem?> UploadBinaryFileAsync(string userId, string fileName, byte[] content)
        {
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
                return null;

            using var stream = new MemoryStream(content);
            var uploadedItem = await GraphClient.Drives[drive.Id].Items["root"]
                .ItemWithPath(fileName)
                .Content
                .PutAsync(stream);

            return uploadedItem;
        }

        /// <summary>
        /// Searches for files in OneDrive.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="maxItems">Maximum number of items to retrieve.</param>
        /// <returns>List of drive items matching the search.</returns>
        public async Task<List<DriveItem>> SearchFilesAsync(string userId, string searchQuery, int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
                return new List<DriveItem>();

            // Search using query parameters
            var items = await GraphClient.Drives[drive.Id].Items["root"].Children.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Search = $"\"{searchQuery}\"";
                requestConfiguration.QueryParameters.Top = itemsToRetrieve;
            });

            return items?.Value?.ToList() ?? new List<DriveItem>();
        }

        /// <summary>
        /// Gets a summary of OneDrive files that fits within context window constraints.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="maxItems">Maximum number of items to retrieve.</param>
        /// <returns>A formatted string summary of files.</returns>
        public async Task<string> GetFilesSummaryAsync(string userId = "me", int? maxItems = null)
        {
            var files = await GetFilesAsync(userId, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {files.Count} items in OneDrive:");

            foreach (var file in files)
            {
                var type = file.Folder != null ? "Folder" : "File";
                summary.AppendLine($"- {file.Name} ({type}, Size: {file.Size} bytes, Modified: {file.LastModifiedDateTime})");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many items ({files.Count}) to display full summary. Showing first 10:");
                foreach (var file in files.Take(10))
                {
                    var type = file.Folder != null ? "Folder" : "File";
                    summary.AppendLine($"- {file.Name} ({type}, Size: {file.Size} bytes)");
                }
            }

            return summary.ToString();
        }
    }
}
