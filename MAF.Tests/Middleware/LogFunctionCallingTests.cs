using Xunit;
using MAF.Assistants.Middleware;
using System;
using System.Linq;
using System.Reflection;

namespace MAF.Tests.Middleware
{
    public class LogFunctionCallingTests
    {
        [Fact]
        public void LogFunctionCalling_IsInternalClass()
        {
            var type = typeof(LogFunctionCalling);
            Assert.False(type.IsPublic);
            Assert.True(type.IsNotPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void LogFunctionCallAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(LogFunctionCalling).GetMethod("LogFunctionCallAsync", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(4, parameters.Length);
            Assert.Equal("callingAgent", parameters[0].Name);
            Assert.Equal("context", parameters[1].Name);
            Assert.Equal("next", parameters[2].Name);
            Assert.Equal("cancellationToken", parameters[3].Name);
        }

        [Fact]
        public void LogFunctionCalling_HasOnePublicMethod()
        {
            var publicMethods = typeof(LogFunctionCalling)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.DeclaringType == typeof(LogFunctionCalling))
                .ToList();

            Assert.Single(publicMethods);
            Assert.Equal("LogFunctionCallAsync", publicMethods[0].Name);
        }
    }
}
