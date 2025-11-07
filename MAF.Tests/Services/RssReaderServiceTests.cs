using Xunit;
using MAF.Assistants.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAF.Tests.Services
{
    /// <summary>
    /// Tests for RssReaderService
    /// Note: These tests validate the service structure and method signatures.
    /// </summary>
    public class RssReaderServiceTests
    {
        [Fact]
        public void RssReaderService_CanBeInstantiated()
        {
            var service = new RssReaderService();
            Assert.NotNull(service);
        }

        [Fact]
        public void ReadFeedAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(RssReaderService).GetMethod("ReadFeedAsync", new[] { typeof(string) });
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            Assert.Equal(typeof(Task<>), methodInfo.ReturnType.GetGenericTypeDefinition());
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("feedUrl", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
        }

        [Fact]
        public void ReadFeedAsync_WithMaxItems_HasCorrectSignature()
        {
            var methodInfo = typeof(RssReaderService).GetMethod("ReadFeedAsync", new[] { typeof(string), typeof(int) });
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            Assert.Equal(typeof(Task<>), methodInfo.ReturnType.GetGenericTypeDefinition());
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("feedUrl", parameters[0].Name);
            Assert.Equal("maxItems", parameters[1].Name);
        }

        [Fact]
        public async Task ReadFeedAsync_ThrowsArgumentException_WhenFeedUrlIsNull()
        {
            var service = new RssReaderService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.ReadFeedAsync(null!));
        }

        [Fact]
        public async Task ReadFeedAsync_ThrowsArgumentException_WhenFeedUrlIsEmpty()
        {
            var service = new RssReaderService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.ReadFeedAsync(string.Empty));
        }

        [Fact]
        public async Task ReadFeedAsync_ThrowsArgumentException_WhenFeedUrlIsWhitespace()
        {
            var service = new RssReaderService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.ReadFeedAsync("   "));
        }

        [Fact]
        public void RssItem_HasAllRequiredProperties()
        {
            var rssItem = new RssItem();
            
            Assert.NotNull(rssItem.Title);
            Assert.NotNull(rssItem.Content);
            Assert.NotNull(rssItem.Excerpt);
            Assert.NotNull(rssItem.Author);
            Assert.NotNull(rssItem.ReferencedUrls);
            Assert.NotNull(rssItem.Link);
        }

        [Fact]
        public void RssItem_Title_CanBeSetAndGet()
        {
            var rssItem = new RssItem { Title = "Test Title" };
            Assert.Equal("Test Title", rssItem.Title);
        }

        [Fact]
        public void RssItem_PublishedDate_CanBeSetAndGet()
        {
            var date = DateTimeOffset.Now;
            var rssItem = new RssItem { PublishedDate = date };
            Assert.Equal(date, rssItem.PublishedDate);
        }

        [Fact]
        public void RssItem_Content_CanBeSetAndGet()
        {
            var rssItem = new RssItem { Content = "Test Content" };
            Assert.Equal("Test Content", rssItem.Content);
        }

        [Fact]
        public void RssItem_Excerpt_CanBeSetAndGet()
        {
            var rssItem = new RssItem { Excerpt = "Test Excerpt" };
            Assert.Equal("Test Excerpt", rssItem.Excerpt);
        }

        [Fact]
        public void RssItem_Author_CanBeSetAndGet()
        {
            var rssItem = new RssItem { Author = "Test Author" };
            Assert.Equal("Test Author", rssItem.Author);
        }

        [Fact]
        public void RssItem_ReferencedUrls_CanBeSetAndGet()
        {
            var urls = new List<string> { "https://example.com", "https://test.com" };
            var rssItem = new RssItem { ReferencedUrls = urls };
            Assert.Equal(urls, rssItem.ReferencedUrls);
        }

        [Fact]
        public void RssItem_Link_CanBeSetAndGet()
        {
            var rssItem = new RssItem { Link = "https://example.com/article" };
            Assert.Equal("https://example.com/article", rssItem.Link);
        }

        [Fact]
        public void RssReaderService_HasAllExpectedPublicMethods()
        {
            var expectedMethods = new[]
            {
                "ReadFeedAsync"
            };

            var publicMethods = typeof(RssReaderService)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.DeclaringType == typeof(RssReaderService))
                .Select(m => m.Name)
                .Distinct()
                .ToList();

            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, publicMethods);
            }
        }

        [Fact]
        public void RssItem_DefaultValues_AreCorrect()
        {
            var rssItem = new RssItem();
            
            Assert.Equal(string.Empty, rssItem.Title);
            Assert.Null(rssItem.PublishedDate);
            Assert.Equal(string.Empty, rssItem.Content);
            Assert.Equal(string.Empty, rssItem.Excerpt);
            Assert.Equal(string.Empty, rssItem.Author);
            Assert.Empty(rssItem.ReferencedUrls);
            Assert.Equal(string.Empty, rssItem.Link);
        }

        [Fact]
        public void RssItem_ReferencedUrls_InitializesToEmptyList()
        {
            var rssItem = new RssItem();
            Assert.NotNull(rssItem.ReferencedUrls);
            Assert.IsType<List<string>>(rssItem.ReferencedUrls);
            Assert.Empty(rssItem.ReferencedUrls);
        }
    }
}
