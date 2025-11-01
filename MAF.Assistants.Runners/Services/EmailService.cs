using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Services
{
    /// <summary>
    /// Service for email operations via Microsoft Graph.
    /// </summary>
    public class EmailService : GraphService
    {
        /// <summary>
        /// Gets emails from a user's mailbox.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN.</param>
        /// <param name="folderName">The folder to read from (default: Inbox).</param>
        /// <param name="maxItems">Maximum number of emails to retrieve (optional, uses config default if not specified).</param>
        /// <returns>List of messages.</returns>
        public async Task<List<Message>> GetEmailsAsync(string userPrincipalName, string folderName = "Inbox", int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var messages = await GraphClient.Users[userPrincipalName]
                .MailFolders[folderName]
                .Messages
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = itemsToRetrieve;
                    requestConfiguration.QueryParameters.Orderby = new[] { "receivedDateTime DESC" };
                });

            return messages?.Value?.ToList() ?? new List<Message>();
        }

        /// <summary>
        /// Gets a summary of emails that fits within context window constraints.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN.</param>
        /// <param name="folderName">The folder to read from (default: Inbox).</param>
        /// <param name="maxItems">Maximum number of emails to retrieve.</param>
        /// <returns>A formatted string summary of emails.</returns>
        public async Task<string> GetEmailsSummaryAsync(string userPrincipalName, string folderName = "Inbox", int? maxItems = null)
        {
            var emails = await GetEmailsAsync(userPrincipalName, folderName, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {emails.Count} emails in {folderName}:");

            foreach (var email in emails)
            {
                var from = email.From?.EmailAddress?.Address ?? "Unknown";
                var subject = email.Subject ?? "(No Subject)";
                var received = email.ReceivedDateTime?.ToString("yyyy-MM-dd HH:mm") ?? "Unknown";
                summary.AppendLine($"- From: {from}, Subject: {subject}, Received: {received}");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many emails ({emails.Count}) to display full summary. Showing first 10:");
                foreach (var email in emails.Take(10))
                {
                    var from = email.From?.EmailAddress?.Address ?? "Unknown";
                    var subject = email.Subject ?? "(No Subject)";
                    summary.AppendLine($"- From: {from}, Subject: {subject}");
                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Gets the content of a specific email.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <returns>The email message object with full details.</returns>
        public async Task<Message?> GetEmailByIdAsync(string userPrincipalName, string messageId)
        {
            return await GraphClient.Users[userPrincipalName].Messages[messageId].GetAsync();
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN (sender).</param>
        /// <param name="toRecipients">List of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <param name="isHtml">Whether the body is HTML (default: false).</param>
        /// <returns>Task representing the async operation.</returns>
        public async Task SendEmailAsync(string userPrincipalName, List<string> toRecipients, string subject, string body, bool isHtml = false)
        {
            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = isHtml ? BodyType.Html : BodyType.Text,
                    Content = body
                },
                ToRecipients = toRecipients.Select(email => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = email
                    }
                }).ToList()
            };

            var requestBody = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            await GraphClient.Users[userPrincipalName].SendMail.PostAsync(requestBody);
        }

        /// <summary>
        /// Gets mail folders for a user.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN.</param>
        /// <returns>List of mail folders.</returns>
        public async Task<List<MailFolder>> GetMailFoldersAsync(string userPrincipalName)
        {
            var folders = await GraphClient.Users[userPrincipalName].MailFolders.GetAsync();
            return folders?.Value?.ToList() ?? new List<MailFolder>();
        }
    }
}
