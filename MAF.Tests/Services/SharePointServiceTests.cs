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
        public void SharePointService_HasAllExpectedPublicMethods()
        {
            var expectedMethods = new[]
            {
                "GetFilesAsync",
                "GetFileContentAsync",
                "UploadFileAsync",
                "GetSitesAsync",
                "GetDrivesAsync",
                "GetFilesSummaryAsync"
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
