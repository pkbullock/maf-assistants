using Xunit;
using MAF.Assistants.Utility;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MAF.Tests.Utility
{
    public class PersistenceTests
    {
        [Fact]
        public void Persistence_IsStaticClass()
        {
            var type = typeof(Persistence);
            Assert.True(type.IsAbstract && type.IsSealed); // Static classes are abstract and sealed
        }

        [Fact]
        public void SaveConversationToFileAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(Persistence).GetMethod("SaveConversationToFileAsync");
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(Task), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("thread", parameters[0].Name);
            Assert.Equal("fileName", parameters[1].Name);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
        }

        [Fact]
        public void LoadConversationFromFileAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(Persistence).GetMethod("LoadConversationFromFileAsync");
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            Assert.Equal(typeof(Task<>), methodInfo.ReturnType.GetGenericTypeDefinition());
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("agent", parameters[0].Name);
            Assert.Equal("fileName", parameters[1].Name);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
        }

        [Fact]
        public void SaveConversationToFileAsync_HasDefaultFileName()
        {
            var methodInfo = typeof(Persistence).GetMethod("SaveConversationToFileAsync");
            var parameters = methodInfo.GetParameters();
            var fileNameParam = parameters[1];
            
            Assert.True(fileNameParam.HasDefaultValue);
            Assert.Equal("agent_thread.json", fileNameParam.DefaultValue);
        }

        [Fact]
        public void LoadConversationFromFileAsync_HasDefaultFileName()
        {
            var methodInfo = typeof(Persistence).GetMethod("LoadConversationFromFileAsync");
            var parameters = methodInfo.GetParameters();
            var fileNameParam = parameters[1];
            
            Assert.True(fileNameParam.HasDefaultValue);
            Assert.Equal("agent_thread.json", fileNameParam.DefaultValue);
        }

        [Fact]
        public void Persistence_HasTwoPublicMethods()
        {
            var publicMethods = typeof(Persistence)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.DeclaringType == typeof(Persistence))
                .ToList();

            Assert.Equal(2, publicMethods.Count);
            Assert.Contains(publicMethods, m => m.Name == "SaveConversationToFileAsync");
            Assert.Contains(publicMethods, m => m.Name == "LoadConversationFromFileAsync");
        }
    }
}
