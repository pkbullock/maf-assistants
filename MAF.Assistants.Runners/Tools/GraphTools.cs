using MAF.Assistants.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MAF.Assistants.Tools
{
    /// <summary>
    /// Tools for Microsoft Graph operations that can be used by AI agents.
    /// </summary>
    public static class GraphTools
    {
        private static readonly SharePointService _sharePointService = new SharePointService();
        private static readonly EmailService _emailService = new EmailService();

        /// <summary>
        /// Lists SharePoint sites available to the application.
        /// </summary>
        /// <param name="maxSites">Maximum number of sites to retrieve (default: 10).</param>
        /// <returns>A formatted list of SharePoint sites.</returns>
        [Description("Lists SharePoint sites available to the application. Returns site names and IDs.")]
        public static async Task<string> ListSharePointSites([Description("Maximum number of sites to retrieve")] int maxSites = 10)
        {
            try
            {
                var sites = await _sharePointService.GetSitesAsync(maxSites);
                if (!sites.Any())
                    return "No SharePoint sites found.";

                var result = $"Found {sites.Count} SharePoint sites:\n";
                foreach (var site in sites)
                {
                    result += $"- Name: {site.DisplayName}, ID: {site.Id}, URL: {site.WebUrl}\n";
                }
                return result;
            }
            catch (Exception ex)
            {
                return $"Error listing SharePoint sites: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets files from a SharePoint document library.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="driveId">The drive (document library) ID.</param>
        /// <param name="maxFiles">Maximum number of files to retrieve (default: 20).</param>
        /// <returns>A summary of files in the library.</returns>
        [Description("Gets files from a SharePoint document library. Returns file names, sizes, and last modified dates.")]
        public static async Task<string> GetSharePointFiles(
            [Description("The SharePoint site ID")] string siteId,
            [Description("The drive (document library) ID")] string driveId,
            [Description("Maximum number of files to retrieve")] int maxFiles = 20)
        {
            try
            {
                var summary = await _sharePointService.GetFilesSummaryAsync(siteId, driveId, maxFiles);
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error getting SharePoint files: {ex.Message}";
            }
        }

        /// <summary>
        /// Reads the content of a file from SharePoint.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="driveId">The drive (document library) ID.</param>
        /// <param name="itemId">The file item ID.</param>
        /// <returns>The file content as text.</returns>
        [Description("Reads the content of a text file from SharePoint.")]
        public static async Task<string> ReadSharePointFile(
            [Description("The SharePoint site ID")] string siteId,
            [Description("The drive (document library) ID")] string driveId,
            [Description("The file item ID")] string itemId)
        {
            try
            {
                var content = await _sharePointService.GetFileContentAsync(siteId, driveId, itemId);
                return string.IsNullOrEmpty(content) ? "File is empty or could not be read." : content;
            }
            catch (Exception ex)
            {
                return $"Error reading SharePoint file: {ex.Message}";
            }
        }

        /// <summary>
        /// Writes or updates a file in SharePoint.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="driveId">The drive (document library) ID.</param>
        /// <param name="fileName">The name of the file to create or update.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <returns>Confirmation message with file details.</returns>
        [Description("Writes or updates a text file in SharePoint. Creates a new file or overwrites an existing one.")]
        public static async Task<string> WriteSharePointFile(
            [Description("The SharePoint site ID")] string siteId,
            [Description("The drive (document library) ID")] string driveId,
            [Description("The name of the file to create or update")] string fileName,
            [Description("The content to write to the file")] string content)
        {
            try
            {
                var item = await _sharePointService.UploadFileAsync(siteId, driveId, fileName, content);
                return $"Successfully wrote file '{fileName}' to SharePoint. Item ID: {item?.Id}";
            }
            catch (Exception ex)
            {
                return $"Error writing SharePoint file: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets recent emails from a user's inbox.
        /// </summary>
        /// <param name="userEmail">The user's email address.</param>
        /// <param name="maxEmails">Maximum number of emails to retrieve (default: 10).</param>
        /// <returns>A summary of recent emails.</returns>
        [Description("Gets recent emails from a user's inbox. Returns sender, subject, and received date for each email.")]
        public static async Task<string> GetRecentEmails(
            [Description("The user's email address")] string userEmail,
            [Description("Maximum number of emails to retrieve")] int maxEmails = 10)
        {
            try
            {
                var summary = await _emailService.GetEmailsSummaryAsync(userEmail, "Inbox", maxEmails);
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error getting emails: {ex.Message}";
            }
        }

        /// <summary>
        /// Reads the full content of a specific email.
        /// </summary>
        /// <param name="userEmail">The user's email address.</param>
        /// <param name="messageId">The ID of the message to read.</param>
        /// <returns>The full email content including subject, sender, and body.</returns>
        [Description("Reads the full content of a specific email by its ID.")]
        public static async Task<string> ReadEmail(
            [Description("The user's email address")] string userEmail,
            [Description("The ID of the message to read")] string messageId)
        {
            try
            {
                var message = await _emailService.GetEmailByIdAsync(userEmail, messageId);
                if (message == null)
                    return "Email not found.";

                var from = message.From?.EmailAddress?.Address ?? "Unknown";
                var subject = message.Subject ?? "(No Subject)";
                var body = message.Body?.Content ?? "(No Content)";
                var received = message.ReceivedDateTime?.ToString("yyyy-MM-dd HH:mm") ?? "Unknown";

                return $"From: {from}\nSubject: {subject}\nReceived: {received}\n\nBody:\n{body}";
            }
            catch (Exception ex)
            {
                return $"Error reading email: {ex.Message}";
            }
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="senderEmail">The sender's email address.</param>
        /// <param name="recipientEmails">Comma-separated list of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <returns>Confirmation message.</returns>
        [Description("Sends an email to one or more recipients. Requires human confirmation before sending.")]
        public static async Task<string> SendEmail(
            [Description("The sender's email address")] string senderEmail,
            [Description("Comma-separated list of recipient email addresses")] string recipientEmails,
            [Description("Email subject")] string subject,
            [Description("Email body content")] string body)
        {
            try
            {
                var recipients = recipientEmails.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Where(e => IsValidEmail(e))
                    .ToList();

                if (!recipients.Any())
                    return "Error: No valid recipients provided. Please provide valid email addresses.";

                await _emailService.SendEmailAsync(senderEmail, recipients, subject, body);
                return $"Successfully sent email to {string.Join(", ", recipients)}";
            }
            catch (Exception ex)
            {
                return $"Error sending email: {ex.Message}";
            }
        }

        /// <summary>
        /// Validates if a string is a valid email address format.
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // New services for extended capabilities
        private static readonly TeamsService _teamsService = new TeamsService();
        private static readonly OneDriveService _oneDriveService = new OneDriveService();
        private static readonly PlannerService _plannerService = new PlannerService();

        /// <summary>
        /// Lists Microsoft Teams that the user is a member of.
        /// </summary>
        /// <param name="maxTeams">Maximum number of teams to retrieve (default: 10).</param>
        /// <returns>A summary of Teams.</returns>
        [Description("Lists Microsoft Teams that the user is a member of.")]
        public static async Task<string> ListTeams([Description("Maximum number of teams to retrieve")] int maxTeams = 10)
        {
            try
            {
                var summary = await _teamsService.GetTeamsSummaryAsync(maxTeams);
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error listing teams: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets recent messages from a Teams channel.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="maxMessages">Maximum number of messages to retrieve (default: 20).</param>
        /// <returns>A summary of channel messages.</returns>
        [Description("Gets recent messages from a Microsoft Teams channel.")]
        public static async Task<string> GetTeamsChannelMessages(
            [Description("The team ID")] string teamId,
            [Description("The channel ID")] string channelId,
            [Description("Maximum number of messages to retrieve")] int maxMessages = 20)
        {
            try
            {
                var summary = await _teamsService.GetChannelMessagesSummaryAsync(teamId, channelId, maxMessages);
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error getting channel messages: {ex.Message}";
            }
        }

        /// <summary>
        /// Sends a message to a Teams channel.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>Confirmation message.</returns>
        [Description("Sends a message to a Microsoft Teams channel. Requires human confirmation.")]
        public static async Task<string> SendTeamsMessage(
            [Description("The team ID")] string teamId,
            [Description("The channel ID")] string channelId,
            [Description("The message to send")] string message)
        {
            try
            {
                var result = await _teamsService.SendChannelMessageAsync(teamId, channelId, message);
                return result?.Id != null ? $"Message sent successfully. ID: {result.Id}" : "Failed to send message.";
            }
            catch (Exception ex)
            {
                return $"Error sending Teams message: {ex.Message}";
            }
        }

        /// <summary>
        /// Lists files in the user's OneDrive.
        /// </summary>
        /// <param name="maxFiles">Maximum number of files to retrieve (default: 20).</param>
        /// <returns>A summary of OneDrive files.</returns>
        [Description("Lists files in the user's OneDrive root folder.")]
        public static async Task<string> ListOneDriveFiles([Description("Maximum number of files to retrieve")] int maxFiles = 20)
        {
            try
            {
                var summary = await _oneDriveService.GetFilesSummaryAsync("me", maxFiles);
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error listing OneDrive files: {ex.Message}";
            }
        }

        /// <summary>
        /// Reads content from a OneDrive file.
        /// </summary>
        /// <param name="itemId">The file item ID.</param>
        /// <returns>The file content.</returns>
        [Description("Reads the content of a text file from OneDrive.")]
        public static async Task<string> ReadOneDriveFile([Description("The file item ID")] string itemId)
        {
            try
            {
                var content = await _oneDriveService.GetFileContentAsync("me", itemId);
                return string.IsNullOrEmpty(content) ? "File is empty or could not be read." : content;
            }
            catch (Exception ex)
            {
                return $"Error reading OneDrive file: {ex.Message}";
            }
        }

        /// <summary>
        /// Uploads a file to OneDrive.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="content">The file content.</param>
        /// <returns>Confirmation message.</returns>
        [Description("Uploads a text file to OneDrive. Requires human confirmation.")]
        public static async Task<string> UploadToOneDrive(
            [Description("The name of the file to create")] string fileName,
            [Description("The file content")] string content)
        {
            try
            {
                var item = await _oneDriveService.UploadFileAsync("me", fileName, content);
                return $"Successfully uploaded file '{fileName}' to OneDrive. Item ID: {item?.Id}";
            }
            catch (Exception ex)
            {
                return $"Error uploading to OneDrive: {ex.Message}";
            }
        }

        /// <summary>
        /// Lists Planner plans accessible to the user.
        /// </summary>
        /// <param name="maxPlans">Maximum number of plans to retrieve (default: 10).</param>
        /// <returns>A summary of Planner plans.</returns>
        [Description("Lists Microsoft Planner plans accessible to the user.")]
        public static async Task<string> ListPlannerPlans([Description("Maximum number of plans to retrieve")] int maxPlans = 10)
        {
            try
            {
                var summary = await _plannerService.GetPlansSummaryAsync("me", maxPlans);
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error listing Planner plans: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets tasks assigned to the user.
        /// </summary>
        /// <returns>A summary of user's tasks.</returns>
        [Description("Gets tasks assigned to the user in Microsoft Planner.")]
        public static async Task<string> GetMyPlannerTasks()
        {
            try
            {
                var summary = await _plannerService.GetUserTasksSummaryAsync("me");
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error getting Planner tasks: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a new task in a Planner plan.
        /// </summary>
        /// <param name="planId">The plan ID.</param>
        /// <param name="bucketId">The bucket ID.</param>
        /// <param name="title">The task title.</param>
        /// <returns>Confirmation message.</returns>
        [Description("Creates a new task in a Microsoft Planner plan. Requires human confirmation.")]
        public static async Task<string> CreatePlannerTask(
            [Description("The plan ID")] string planId,
            [Description("The bucket ID")] string bucketId,
            [Description("The task title")] string title)
        {
            try
            {
                var task = await _plannerService.CreateTaskAsync(planId, bucketId, title);
                return task?.Id != null ? $"Task created successfully. ID: {task.Id}" : "Failed to create task.";
            }
            catch (Exception ex)
            {
                return $"Error creating Planner task: {ex.Message}";
            }
        }

        /// <summary>
        /// Lists SharePoint lists from a site.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="maxLists">Maximum number of lists to retrieve (default: 20).</param>
        /// <returns>A summary of SharePoint lists.</returns>
        [Description("Lists SharePoint lists from a site. Returns list names and IDs.")]
        public static async Task<string> ListSharePointLists(
            [Description("The SharePoint site ID")] string siteId,
            [Description("Maximum number of lists to retrieve")] int maxLists = 20)
        {
            try
            {
                var summary = await _sharePointService.GetListsSummaryAsync(siteId, maxLists);
                return summary;
            }
            catch (Exception ex)
            {
                return $"Error listing SharePoint lists: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets items from a SharePoint list with optional filtering.
        /// </summary>
        /// <param name="siteId">The SharePoint site ID.</param>
        /// <param name="listId">The list ID.</param>
        /// <param name="maxItems">Maximum number of items to retrieve (default: 50).</param>
        /// <param name="filter">Optional OData filter string (e.g., "fields/Status eq 'Active'").</param>
        /// <param name="fieldNames">Optional comma-separated list of field names to include in the output.</param>
        /// <returns>SharePoint list items formatted as a markdown table.</returns>
        [Description("Gets items from a SharePoint list and returns them as a markdown table. Supports filtering and field selection.")]
        public static async Task<string> GetSharePointListItems(
            [Description("The SharePoint site ID")] string siteId,
            [Description("The list ID")] string listId,
            [Description("Maximum number of items to retrieve")] int maxItems = 50,
            [Description("Optional OData filter string")] string? filter = null,
            [Description("Optional comma-separated list of field names to include")] string? fieldNames = null)
        {
            try
            {
                var items = await _sharePointService.GetListItemsAsync(siteId, listId, maxItems, filter);
                
                if (!items.Any())
                    return "No items found in the list.";

                // Parse field names if provided
                List<string>? fields = null;
                if (!string.IsNullOrEmpty(fieldNames))
                {
                    fields = fieldNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(f => f.Trim())
                        .ToList();
                }

                var markdownTable = _sharePointService.ConvertListItemsToMarkdownTable(items, fields);
                return markdownTable;
            }
            catch (Exception ex)
            {
                return $"Error getting SharePoint list items: {ex.Message}";
            }
        }
    }
}
