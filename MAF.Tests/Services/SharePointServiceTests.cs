using Xunit;
using MAF.Assistants.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MAF.Tests.Services
{
    /// <summary>
    /// Tests for SharePointService
    /// </summary>
    public class SharePointServiceTests
    {
        [Fact]
        public void SharePointService_InheritsFromGraphService()
        {
            var baseType = typeof(SharePointService).BaseType;
            Assert.NotNull(baseType);
            Assert.Equal(typeof(GraphService), baseType);
        }

        [Fact]
        public void GetFilesAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetFilesAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("driveId", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void GetFileContentAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetFileContentAsync");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("driveId", parameters[1].Name);
            Assert.Equal("itemId", parameters[2].Name);
        }

        [Fact]
        public void UploadFileAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("UploadFileAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(4, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("driveId", parameters[1].Name);
            Assert.Equal("fileName", parameters[2].Name);
            Assert.Equal("content", parameters[3].Name);
        }

        [Fact]
        public void GetSitesAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetSitesAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxItems", parameters[0].Name);
        }

        [Fact]
        public void GetDrivesAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetDrivesAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("siteId", parameters[0].Name);
        }

        [Fact]
        public void GetFilesSummaryAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetFilesSummaryAsync");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("driveId", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void GetListsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetListsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("maxItems", parameters[1].Name);
        }

        [Fact]
        public void GetListItemsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetListItemsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(5, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("listId", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
            Assert.Equal("filter", parameters[3].Name);
            Assert.Equal("expand", parameters[4].Name);
        }

        [Fact]
        public void ConvertListItemsToMarkdownTable_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("ConvertListItemsToMarkdownTable");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(string), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("items", parameters[0].Name);
            Assert.Equal("fieldNames", parameters[1].Name);
        }

        [Fact]
        public void GetListsSummaryAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(SharePointService).GetMethod("GetListsSummaryAsync");
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(Task<string>), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("siteId", parameters[0].Name);
            Assert.Equal("maxItems", parameters[1].Name);
        }

        [Fact]
        public void SharePointService_HasAllExpectedPublicMethods()
        {
            var expectedMethods = new[]
            {
                "GetFilesAsync",
                "GetFileContentAsync",
                "UploadFileAsync",
                "GetSitesAsync",
                "GetDrivesAsync",
                "GetFilesSummaryAsync",
                "GetListsAsync",
                "GetListItemsAsync",
                "ConvertListItemsToMarkdownTable",
                "GetListsSummaryAsync"
            };

            var publicMethods = typeof(SharePointService)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.DeclaringType == typeof(SharePointService))
                .Select(m => m.Name)
                .ToList();

            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, publicMethods);
            }
        }
    }
}
