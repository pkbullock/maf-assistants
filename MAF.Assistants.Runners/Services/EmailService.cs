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

        /// <summary>
        /// Gets attachments from an email message.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN.</param>
        /// <param name="messageId">The message ID.</param>
        /// <returns>List of attachments.</returns>
        public async Task<List<Attachment>> GetAttachmentsAsync(string userPrincipalName, string messageId)
        {
            var attachments = await GraphClient.Users[userPrincipalName].Messages[messageId].Attachments.GetAsync();
            return attachments?.Value?.ToList() ?? new List<Attachment>();
        }

        /// <summary>
        /// Gets a specific attachment's content.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN.</param>
        /// <param name="messageId">The message ID.</param>
        /// <param name="attachmentId">The attachment ID.</param>
        /// <returns>The attachment object with content.</returns>
        public async Task<Attachment?> GetAttachmentByIdAsync(string userPrincipalName, string messageId, string attachmentId)
        {
            return await GraphClient.Users[userPrincipalName].Messages[messageId].Attachments[attachmentId].GetAsync();
        }

        /// <summary>
        /// Sends an email with attachments.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN (sender).</param>
        /// <param name="toRecipients">List of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <param name="attachments">List of file attachments (name, content bytes).</param>
        /// <param name="isHtml">Whether the body is HTML (default: false).</param>
        /// <returns>Task representing the async operation.</returns>
        public async Task SendEmailWithAttachmentsAsync(
            string userPrincipalName, 
            List<string> toRecipients, 
            string subject, 
            string body, 
            List<(string Name, byte[] Content)> attachments,
            bool isHtml = false)
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
                }).ToList(),
                Attachments = attachments.Select(att => new FileAttachment
                {
                    Name = att.Name,
                    ContentBytes = att.Content
                } as Attachment).ToList()
            };

            var requestBody = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            await GraphClient.Users[userPrincipalName].SendMail.PostAsync(requestBody);
        }

        /// <summary>
        /// Sends an HTML email with inline images.
        /// </summary>
        /// <param name="userPrincipalName">The user's email address or UPN (sender).</param>
        /// <param name="toRecipients">List of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlBody">HTML body content with &lt;img&gt; tags referencing contentId.</param>
        /// <param name="inlineImages">List of inline images (contentId, name, content bytes).</param>
        /// <returns>Task representing the async operation.</returns>
        public async Task SendHtmlEmailWithInlineImagesAsync(
            string userPrincipalName,
            List<string> toRecipients,
            string subject,
            string htmlBody,
            List<(string ContentId, string Name, byte[] Content)> inlineImages)
        {
            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = htmlBody
                },
                ToRecipients = toRecipients.Select(email => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = email
                    }
                }).ToList(),
                Attachments = inlineImages.Select(img => new FileAttachment
                {
                    Name = img.Name,
                    ContentId = img.ContentId,
                    ContentBytes = img.Content,
                    IsInline = true
                } as Attachment).ToList()
            };

            var requestBody = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            await GraphClient.Users[userPrincipalName].SendMail.PostAsync(requestBody);
        }
    }
}
