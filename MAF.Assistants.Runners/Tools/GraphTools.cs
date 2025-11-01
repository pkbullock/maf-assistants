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
    }
}
