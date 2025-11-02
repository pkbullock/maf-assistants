using Xunit;
using MAF.Assistants.Services;
using System.Linq;

namespace MAF.Tests.Services
{
    public class TeamsServiceTests
    {
        [Fact]
        public void TeamsService_InheritsFromGraphService()
        {
            var baseType = typeof(TeamsService).BaseType;
            Assert.NotNull(baseType);
            Assert.Equal(typeof(GraphService), baseType);
        }

        [Fact]
        public void GetTeamsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(TeamsService).GetMethod("GetTeamsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("maxItems", parameters[0].Name);
        }

        [Fact]
        public void GetChannelsAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(TeamsService).GetMethod("GetChannelsAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("teamId", parameters[0].Name);
        }

        [Fact]
        public void GetChannelMessagesAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(TeamsService).GetMethod("GetChannelMessagesAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(3, parameters.Length);
            Assert.Equal("teamId", parameters[0].Name);
            Assert.Equal("channelId", parameters[1].Name);
            Assert.Equal("maxItems", parameters[2].Name);
        }

        [Fact]
        public void TeamsService_HasExpectedPublicMethods()
        {
            var publicMethods = typeof(TeamsService)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.DeclaringType == typeof(TeamsService))
                .Select(m => m.Name)
                .ToList();

            Assert.Contains("GetTeamsAsync", publicMethods);
            Assert.Contains("GetChannelsAsync", publicMethods);
        }
    }
}
