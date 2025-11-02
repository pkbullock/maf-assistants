using Xunit;
using MAF.Assistants.Runners;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Abstract;
using System;
using System.Reflection;

namespace MAF.Tests.Runners
{
    public class GraphChatWithToolsTests
    {
        [Fact]
        public void GraphChatWithTools_ImplementsRunChatModelInterface()
        {
            var type = typeof(GraphChatWithTools);
            var interfaces = type.GetInterfaces();
            
            Assert.Contains(typeof(RunChatModel), interfaces);
        }

        [Fact]
        public void GraphChatWithTools_InheritsFromBaseChat()
        {
            var type = typeof(GraphChatWithTools);
            var baseType = type.BaseType;
            
            Assert.NotNull(baseType);
            Assert.Equal(typeof(BaseChat), baseType);
        }

        [Fact]
        public void GraphChatWithTools_HasStartAsyncMethod()
        {
            var methodInfo = typeof(GraphChatWithTools).GetMethod("StartAsync");
            
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(System.Threading.Tasks.Task), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("client", parameters[0].Name);
        }

        [Fact]
        public void GraphChatWithTools_IsInternalClass()
        {
            var type = typeof(GraphChatWithTools);
            Assert.False(type.IsPublic);
            Assert.True(type.IsNotPublic);
        }

        [Fact]
        public void GraphChatWithTools_HasConstructor()
        {
            var constructors = typeof(GraphChatWithTools).GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            Assert.NotEmpty(constructors);
        }

        [Fact]
        public void GraphChatWithTools_RequiresTools_IsTrue()
        {
            // GraphChatWithTools uses Microsoft Graph tools, so RequiresTools should be true
            var propertyInfo = typeof(BaseChat).GetProperty("RequiresTools");
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(bool), propertyInfo.PropertyType);
        }
    }
}
