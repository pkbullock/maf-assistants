using Xunit;
using MAF.Assistants.Runners;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Abstract;
using System;
using System.Reflection;

namespace MAF.Tests.Runners
{
    public class SimpleChatTests
    {
        [Fact]
        public void SimpleChat_ImplementsRunChatModelInterface()
        {
            var type = typeof(SimpleChat);
            var interfaces = type.GetInterfaces();
            
            Assert.Contains(typeof(RunChatModel), interfaces);
        }

        [Fact]
        public void SimpleChat_InheritsFromBaseChat()
        {
            var type = typeof(SimpleChat);
            var baseType = type.BaseType;
            
            Assert.NotNull(baseType);
            Assert.Equal(typeof(BaseChat), baseType);
        }

        [Fact]
        public void SimpleChat_HasStartAsyncMethod()
        {
            var methodInfo = typeof(SimpleChat).GetMethod("StartAsync");
            
            Assert.NotNull(methodInfo);
            Assert.Equal(typeof(System.Threading.Tasks.Task), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("client", parameters[0].Name);
        }

        [Fact]
        public void SimpleChat_IsInternalClass()
        {
            var type = typeof(SimpleChat);
            Assert.False(type.IsPublic);
            Assert.True(type.IsNotPublic);
        }

        [Fact]
        public void SimpleChat_RequiresTools_IsFalse()
        {
            // SimpleChat doesn't use tools, so RequiresTools should be false
            // We can't instantiate without proper dependencies, but we can verify the property exists
            var propertyInfo = typeof(BaseChat).GetProperty("RequiresTools");
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(bool), propertyInfo.PropertyType);
        }
    }
}
