using Xunit;
using MAF.Assistants.Utility;
using System;
using System.Linq;
using System.Reflection;

namespace MAF.Tests.Utility
{
    public class WriteOutTests
    {
        [Fact]
        public void WriteOut_IsInternalClass()
        {
            var type = typeof(WriteOut);
            Assert.False(type.IsPublic);
            Assert.True(type.IsNotPublic);
        }

        [Fact]
        public void Msg_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("Msg", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("input", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
            Assert.Equal("color", parameters[1].Name);
        }

        [Fact]
        public void MsgBlankLine_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("MsgBlankLine", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
            Assert.Empty(methodInfo.GetParameters());
        }

        [Fact]
        public void MsgGrey_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("MsgGrey", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("input", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
        }

        [Fact]
        public void MsgCyan_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("MsgCyan", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
        }

        [Fact]
        public void MsgGreen_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("MsgGreen", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
        }

        [Fact]
        public void MsgYellow_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("MsgYellow", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
        }

        [Fact]
        public void MsgRed_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("MsgRed", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
        }

        [Fact]
        public void EndOfProgram_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("EndOfProgram", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
        }

        [Fact]
        public void Divider_HasCorrectSignature()
        {
            var methodInfo = typeof(WriteOut).GetMethod("Divider", BindingFlags.Public | BindingFlags.Static);
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("noNewLine", parameters[0].Name);
            Assert.Equal(typeof(bool), parameters[0].ParameterType);
        }

        [Fact]
        public void WriteOut_HasAllColoredOutputMethods()
        {
            var expectedMethods = new[] { "MsgGrey", "MsgCyan", "MsgGreen", "MsgYellow", "MsgRed" };
            var publicStaticMethods = typeof(WriteOut)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Select(m => m.Name)
                .ToList();

            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, publicStaticMethods);
            }
        }
    }
}
