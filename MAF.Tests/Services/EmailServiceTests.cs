using Xunit;
using MAF.Assistants.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAF.Tests.Services
{
    /// <summary>
    /// Tests for EmailService
    /// Note: These tests validate the service structure and method signatures.
    /// Full integration tests would require a test Microsoft 365 environment.
    /// </summary>
    public class EmailServiceTests
    {
        [Fact]
        public void EmailService_InheritsFromGraphService()
        {
            var baseType = typeof(EmailService).BaseType;
            Assert.NotNull(baseType);
            Assert.Equal(typeof(GraphService), baseType);
        }

        [Fact]
        public void GetEmailsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("GetEmailsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            Assert.Equal(typeof(Task<>), methodInfo.ReturnType.GetGenericTypeDefinition());
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("folderName", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void GetEmailsSummaryAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("GetEmailsSummaryAsync");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("folderName", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void GetEmailByIdAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("GetEmailByIdAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("messageId", parameters[1].Name);
        }

        [Fact]
        public void SendEmailAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("SendEmailAsync");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(5, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("toRecipients", parameters[1].Name);
            Assert.Equal("subject", parameters[2].Name);
            Assert.Equal("body", parameters[3].Name);
            Assert.Equal("isHtml", parameters[4].Name);
        }

        [Fact]
        public void GetMailFoldersAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("GetMailFoldersAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("userPrincipalName", parameters[0].Name);
        }

        [Fact]
        public void GetAttachmentsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("GetAttachmentsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("messageId", parameters[1].Name);
        }

        [Fact]
        public void GetAttachmentByIdAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("GetAttachmentByIdAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("messageId", parameters[1].Name);
            Assert.Equal("attachmentId", parameters[2].Name);
        }

        [Fact]
        public void SendEmailWithAttachmentsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("SendEmailWithAttachmentsAsync");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(6, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("toRecipients", parameters[1].Name);
            Assert.Equal("subject", parameters[2].Name);
            Assert.Equal("body", parameters[3].Name);
            Assert.Equal("attachments", parameters[4].Name);
            Assert.Equal("isHtml", parameters[5].Name);
        }

        [Fact]
        public void SendHtmlEmailWithInlineImagesAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(EmailService).GetMethod("SendHtmlEmailWithInlineImagesAsync");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(5, parameters.Length);
            Assert.Equal("userPrincipalName", parameters[0].Name);
            Assert.Equal("toRecipients", parameters[1].Name);
            Assert.Equal("subject", parameters[2].Name);
            Assert.Equal("htmlBody", parameters[3].Name);
            Assert.Equal("inlineImages", parameters[4].Name);
        }

        [Fact]
        public void EmailService_HasAllExpectedPublicMethods()
        {
            var expectedMethods = new[]
            {
                "GetEmailsAsync",
                "GetEmailsSummaryAsync",
                "GetEmailByIdAsync",
                "SendEmailAsync",
                "GetMailFoldersAsync",
                "GetAttachmentsAsync",
                "GetAttachmentByIdAsync",
                "SendEmailWithAttachmentsAsync",
                "SendHtmlEmailWithInlineImagesAsync"
            };

            var publicMethods = typeof(EmailService)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.DeclaringType == typeof(EmailService))
                .Select(m => m.Name)
                .ToList();

            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, publicMethods);
            }
        }


        [Fact]
        public async Task EmailService_CallsEmailService_Live()
        {
            EmailService emailService = new EmailService();
            var result = await emailService.GetEmailsAsync("norma.person@pkbmvp.onmicrosoft.com", maxItems: 5);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);

        }
    }
}
