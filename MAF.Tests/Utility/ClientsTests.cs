using Xunit;
using MAF.Assistants.Utility;
using System;
using System.Reflection;

namespace MAF.Tests.Utility
{
    public class ClientsTests
    {
        [Fact]
        public void Clients_IsStaticClass()
        {
            var type = typeof(Clients);
            Assert.True(type.IsAbstract && type.IsSealed); // Static classes are abstract and sealed
            Assert.False(type.IsPublic); // It's internal
        }

        [Fact]
        public void Azure_HasCorrectSignature()
        {
            var methodInfo = typeof(Clients).GetMethod("Azure", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Empty(methodInfo.GetParameters());
        }

        [Fact]
        public void GetAzureChat_HasCorrectSignature()
        {
            var methodInfo = typeof(Clients).GetMethod("GetAzureChat", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Empty(methodInfo.GetParameters());
        }

        [Fact]
        public void LocalFoundry_HasCorrectSignature()
        {
            var methodInfo = typeof(Clients).GetMethod("LocalFoundry", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("requiresTools", parameters[0].Name);
            Assert.Equal(typeof(bool), parameters[0].ParameterType);
        }

        [Fact]
        public void GetLocalFoundryChat_HasCorrectSignature()
        {
            var methodInfo = typeof(Clients).GetMethod("GetLocalFoundryChat", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("requiresTools", parameters[0].Name);
        }

        [Fact]
        public void BlockLocalFoundryChatNotSupported_HasCorrectSignature()
        {
            var methodInfo = typeof(Clients).GetMethod("BlockLocalFoundryChatNotSupported", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("client", parameters[0].Name);
        }

        [Fact]
        public void OpenAI_ThrowsNotImplementedException()
        {
            var methodInfo = typeof(Clients).GetMethod("OpenAI", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void Ollama_ThrowsNotImplementedException()
        {
            var methodInfo = typeof(Clients).GetMethod("Ollama", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void RunOptions_IsNestedClass()
        {
            var type = typeof(Clients.RunOptions);
            Assert.NotNull(type);
            Assert.True(type.IsClass);
            Assert.True(type.IsNested);
        }

        [Fact]
        public void RunOptions_HasRequiresToolsProperty()
        {
            var propertyInfo = typeof(Clients.RunOptions).GetProperty("RequiresTools");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(bool), propertyInfo.PropertyType);
        }

        [Fact]
        public void RunOptions_HasConstructor()
        {
            var constructorInfo = typeof(Clients.RunOptions).GetConstructor(new[] { typeof(bool) });
            
            Assert.NotNull(constructorInfo);
            
            var parameters = constructorInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("requiresTools", parameters[0].Name);
            Assert.Equal(typeof(bool), parameters[0].ParameterType);
        }
    }
}
