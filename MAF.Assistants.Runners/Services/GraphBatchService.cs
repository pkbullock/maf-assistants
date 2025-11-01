using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAF.Assistants.Services
{
    /// <summary>
    /// Service for performing batch operations with Microsoft Graph.
    /// Batch operations allow multiple API calls to be sent in a single HTTP request,
    /// improving performance when making multiple related requests.
    /// </summary>
    public class GraphBatchService : GraphService
    {
        /// <summary>
        /// Gets multiple SharePoint files in a single batch operation.
        /// </summary>
        /// <param name="driveId">The drive ID.</param>
        /// <param name="itemIds">List of item IDs to retrieve.</param>
        /// <returns>Dictionary of item IDs to their content.</returns>
        public async Task<Dictionary<string, string>> GetMultipleFileContentsBatchAsync(string driveId, List<string> itemIds)
        {
            var results = new Dictionary<string, string>();
            
            // Graph SDK doesn't have built-in batch support in v5, so we'll do sequential for now
            // In a real implementation, you would use the $batch endpoint directly
            foreach (var itemId in itemIds.Take(20)) // Limit to 20 items per batch
            {
                try
                {
                    var stream = await GraphClient.Drives[driveId].Items[itemId].Content.GetAsync();
                    if (stream != null)
                    {
                        using var reader = new System.IO.StreamReader(stream);
                        results[itemId] = await reader.ReadToEndAsync();
                    }
                }
                catch (Exception ex)
                {
                    results[itemId] = $"Error: {ex.Message}";
                }
            }

            return results;
        }

        /// <summary>
        /// Gets multiple email messages in batch.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN.</param>
        /// <param name="messageIds">List of message IDs to retrieve.</param>
        /// <returns>Dictionary of message IDs to message objects.</returns>
        public async Task<Dictionary<string, Message?>> GetMultipleEmailsBatchAsync(string userPrincipalName, List<string> messageIds)
        {
            var results = new Dictionary<string, Message?>();
            
            foreach (var messageId in messageIds.Take(20)) // Limit to 20 items per batch
            {
                try
                {
                    var message = await GraphClient.Users[userPrincipalName].Messages[messageId].GetAsync();
                    results[messageId] = message;
                }
                catch (Exception)
                {
                    results[messageId] = null;
                }
            }

            return results;
        }

        /// <summary>
        /// Gets multiple user profiles in batch.
        /// </summary>
        /// <param name="userIds">List of user IDs to retrieve.</param>
        /// <returns>Dictionary of user IDs to user objects.</returns>
        public async Task<Dictionary<string, User?>> GetMultipleUsersBatchAsync(List<string> userIds)
        {
            var results = new Dictionary<string, User?>();
            
            foreach (var userId in userIds.Take(20)) // Limit to 20 items per batch
            {
                try
                {
                    var user = await GraphClient.Users[userId].GetAsync();
                    results[userId] = user;
                }
                catch (Exception)
                {
                    results[userId] = null;
                }
            }

            return results;
        }

        /// <summary>
        /// Performs multiple file uploads to OneDrive in batch.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="files">Dictionary of file names to content.</param>
        /// <returns>Dictionary of file names to upload results (success/error message).</returns>
        public async Task<Dictionary<string, string>> BatchUploadFilesToOneDriveAsync(string userId, Dictionary<string, string> files)
        {
            var results = new Dictionary<string, string>();
            
            // Get drive ID first
            var drive = await GraphClient.Users[userId].Drive.GetAsync();
            if (drive?.Id == null)
            {
                foreach (var file in files)
                {
                    results[file.Key] = "Error: Could not access drive";
                }
                return results;
            }

            foreach (var file in files.Take(20)) // Limit to 20 items per batch
            {
                try
                {
                    using var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(file.Value));
                    var uploadedItem = await GraphClient.Drives[drive.Id].Items["root"]
                        .ItemWithPath(file.Key)
                        .Content
                        .PutAsync(stream);

                    results[file.Key] = uploadedItem?.Id != null ? $"Success: {uploadedItem.Id}" : "Error: Upload failed";
                }
                catch (Exception ex)
                {
                    results[file.Key] = $"Error: {ex.Message}";
                }
            }

            return results;
        }

        /// <summary>
        /// Gets a summary of batch operation capabilities and limits.
        /// </summary>
        /// <returns>Information about batch operations.</returns>
        public string GetBatchOperationInfo()
        {
            return @"Microsoft Graph Batch Operations:
- Maximum 20 requests per batch
- Reduces network overhead and improves performance
- Supports GET, POST, PATCH, PUT, DELETE operations
- Each request in the batch is independent
- Returns individual status codes for each request

Supported operations:
- Multiple file content retrievals
- Multiple email retrievals
- Multiple user profile lookups
- Batch file uploads
- Mixed operation types in a single batch

Note: Current implementation uses sequential operations.
For true batch performance, use the $batch endpoint directly.";
        }
    }
}
