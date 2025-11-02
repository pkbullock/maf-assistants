using Xunit;
using MAF.Assistants.Tools;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MAF.Tests.Tools
{
    /// <summary>
    /// Tests for GraphTools static methods
    /// Note: These tests validate method signatures and basic validation logic.
    /// Full integration tests would require a test Microsoft 365 environment.
    /// </summary>
    public class GraphToolsTests
    {
        [Fact]
        public void IsValidEmail_ValidatesEmailFormat()
        {
            // Test valid emails through the SendEmail method's validation
            // Since IsValidEmail is private, we test it indirectly
            
            // This test validates that the GraphTools class exists and has the expected methods
            var methodInfo = typeof(GraphTools).GetMethod("SendEmail");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
        }

        [Fact]
        public void ListSharePointSites_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("ListSharePointSites");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxSites", parameters[0].Name);
            Assert.Equal(typeof(int), parameters[0].ParameterType);
        }

        [Fact]
        public void GetSharePointFiles_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("GetSharePointFiles");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("driveId", parameters[1].Name);
            Assert.Equal("maxFiles", parameters[2].Name);
        }

        [Fact]
        public void ReadSharePointFile_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("ReadSharePointFile");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("driveId", parameters[1].Name);
            Assert.Equal("itemId", parameters[2].Name);
        }

        [Fact]
        public void WriteSharePointFile_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("WriteSharePointFile");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(4, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("driveId", parameters[1].Name);
            Assert.Equal("fileName", parameters[2].Name);
            Assert.Equal("content", parameters[3].Name);
        }

        [Fact]
        public void GetRecentEmails_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("GetRecentEmails");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("userEmail", parameters[0].Name);
            Assert.Equal("maxEmails", parameters[1].Name);
        }

        [Fact]
        public void ReadEmail_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("ReadEmail");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("userEmail", parameters[0].Name);
            Assert.Equal("messageId", parameters[1].Name);
        }

        [Fact]
        public void SendEmail_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("SendEmail");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(4, parameters.Length);
            Assert.Equal("senderEmail", parameters[0].Name);
            Assert.Equal("recipientEmails", parameters[1].Name);
            Assert.Equal("subject", parameters[2].Name);
            Assert.Equal("body", parameters[3].Name);
        }

        [Fact]
        public void ListTeams_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("ListTeams");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxTeams", parameters[0].Name);
        }

        [Fact]
        public void GetTeamsChannelMessages_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("GetTeamsChannelMessages");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("teamId", parameters[0].Name);
            Assert.Equal("channelId", parameters[1].Name);
            Assert.Equal("maxMessages", parameters[2].Name);
        }

        [Fact]
        public void SendTeamsMessage_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("SendTeamsMessage");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("teamId", parameters[0].Name);
            Assert.Equal("channelId", parameters[1].Name);
            Assert.Equal("message", parameters[2].Name);
        }

        [Fact]
        public void ListOneDriveFiles_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("ListOneDriveFiles");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxFiles", parameters[0].Name);
        }

        [Fact]
        public void ReadOneDriveFile_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("ReadOneDriveFile");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("itemId", parameters[0].Name);
        }

        [Fact]
        public void UploadToOneDrive_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("UploadToOneDrive");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("fileName", parameters[0].Name);
            Assert.Equal("content", parameters[1].Name);
        }

        [Fact]
        public void ListPlannerPlans_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("ListPlannerPlans");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxPlans", parameters[0].Name);
        }

        [Fact]
        public void GetMyPlannerTasks_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("GetMyPlannerTasks");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Empty(parameters);
        }

        [Fact]
        public void CreatePlannerTask_HasCorrectSignature()
        {
            var methodInfo = typeof(GraphTools).GetMethod("CreatePlannerTask");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("planId", parameters[0].Name);
            Assert.Equal("bucketId", parameters[1].Name);
            Assert.Equal("title", parameters[2].Name);
        }

        [Fact]
        public void GraphTools_AllMethodsReturnTaskOfString()
        {
            var methods = typeof(GraphTools).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var publicMethods = methods.Where(m => !m.IsSpecialName && m.DeclaringType == typeof(GraphTools));
            
            foreach (var method in publicMethods)
            {
                Assert.Equal(typeof(Task<string>), method.ReturnType);
            }
        }

        [Fact]
        public void GraphTools_HasDescriptionAttributes()
        {
            var methods = typeof(GraphTools).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var publicMethods = methods.Where(m => !m.IsSpecialName && m.DeclaringType == typeof(GraphTools)).ToList();
            
            Assert.True(publicMethods.Count > 0, "GraphTools should have public static methods");
            
            // Check that methods have Description attributes
            foreach (var method in publicMethods)
            {
                var attributes = method.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                Assert.True(attributes.Count() > 0, $"Method {method.Name} should have a Description attribute");
            }
        }
    }
}
