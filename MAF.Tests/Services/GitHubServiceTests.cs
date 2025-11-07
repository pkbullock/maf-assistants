using Xunit;
using MAF.Assistants.Services;
using System.Linq;

namespace MAF.Tests.Services
{
    public class GitHubServiceTests
    {
        [Fact]
        public void GitHubService_Constructor_DoesNotThrowWithValidConfig()
        {
            // This test verifies the service can be instantiated
            // Note: It will throw if GitHubToken is not configured in user secrets
            // This is expected behavior as it's a live integration test
            try
            {
                var service = new GitHubService();
                Assert.NotNull(service);
            }
            catch (System.InvalidOperationException ex)
            {
                // Expected if GitHubToken is not configured
                Assert.Contains("GitHub configuration is missing", ex.Message);
            }
        }

        [Fact]
        public void ReviewPullRequestsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("ReviewPullRequestsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("owner", parameters[0].Name);
            Assert.Equal("repository", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void ReviewGitHubStarsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("ReviewGitHubStarsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxItems", parameters[0].Name);
        }

        [Fact]
        public void ReviewIssuesAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("ReviewIssuesAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(4, parameters.Length);
            Assert.Equal("owner", parameters[0].Name);
            Assert.Equal("repository", parameters[1].Name);
            Assert.Equal("state", parameters[2].Name);
            Assert.Equal("maxItems", parameters[3].Name);
        }

        [Fact]
        public void ReviewIssueByIdAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("ReviewIssueByIdAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("owner", parameters[0].Name);
            Assert.Equal("repository", parameters[1].Name);
            Assert.Equal("issueNumber", parameters[2].Name);
        }

        [Fact]
        public void TestConnectionAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("TestConnectionAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Empty(parameters);
        }

        [Fact]
        public void GetPullRequestsSummaryAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("GetPullRequestsSummaryAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("owner", parameters[0].Name);
            Assert.Equal("repository", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void GetStarredRepositoriesSummaryAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("GetStarredRepositoriesSummaryAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxItems", parameters[0].Name);
        }

        [Fact]
        public void GetIssuesSummaryAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(GitHubService).GetMethod("GetIssuesSummaryAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(4, parameters.Length);
            Assert.Equal("owner", parameters[0].Name);
            Assert.Equal("repository", parameters[1].Name);
            Assert.Equal("state", parameters[2].Name);
            Assert.Equal("maxItems", parameters[3].Name);
        }

        [Fact]
        public void GitHubService_HasProtectedProperties()
        {
            var gitHubClientProperty = typeof(GitHubService).GetProperty("GitHubClient", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(gitHubClientProperty);

            var maxItemsProperty = typeof(GitHubService).GetProperty("MaxItems",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(maxItemsProperty);
        }
    }
}
