using Xunit;
using MAF.Assistants.Services;
using System.Linq;

namespace MAF.Tests.Services
{
    public class OneDriveServiceTests
    {
        [Fact]
        public void OneDriveService_InheritsFromGraphService()
        {
            var baseType = typeof(OneDriveService).BaseType;
            Assert.NotNull(baseType);
            Assert.Equal(typeof(GraphService), baseType);
        }

        [Fact]
        public void GetFilesAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(OneDriveService).GetMethod("GetFilesAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("userId", parameters[0].Name);
            Assert.Equal("maxItems", parameters[1].Name);
        }

        [Fact]
        public void GetFilesInFolderAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(OneDriveService).GetMethod("GetFilesInFolderAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("userId", parameters[0].Name);
            Assert.Equal("folderId", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void OneDriveService_HasExpectedPublicMethods()
        {
            var expectedMethods = new[] { "GetFilesAsync", "GetFilesInFolderAsync" };
            var publicMethods = typeof(OneDriveService)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.DeclaringType == typeof(OneDriveService))
                .Select(m => m.Name)
                .ToList();

            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, publicMethods);
            }
        }
    }
}
