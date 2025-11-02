using Xunit;
using MAF.Assistants.Middleware;
using System;
using System.Linq;
using System.Reflection;

namespace MAF.Tests.Middleware
{
    public class HumanInTheLoopTests
    {
        [Fact]
        public void HumanInTheLoop_IsPublicClass()
        {
            var type = typeof(HumanInTheLoop);
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void ConfirmSensitiveOperationAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(HumanInTheLoop).GetMethod("ConfirmSensitiveOperationAsync");
            
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
        public void AddSensitiveFunction_HasCorrectSignature()
        {
            var methodInfo = typeof(HumanInTheLoop).GetMethod("AddSensitiveFunction");
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("functionName", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
        }

        [Fact]
        public void RemoveSensitiveFunction_HasCorrectSignature()
        {
            var methodInfo = typeof(HumanInTheLoop).GetMethod("RemoveSensitiveFunction");
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(void), methodInfo.ReturnType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("functionName", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
        }

        [Fact]
        public void GetSensitiveFunctions_HasCorrectSignature()
        {
            var methodInfo = typeof(HumanInTheLoop).GetMethod("GetSensitiveFunctions");
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            Assert.Empty(methodInfo.GetParameters());
        }

        [Fact]
        public void GetSensitiveFunctions_ReturnsReadOnlyCollection()
        {
            var sensitiveFunctions = HumanInTheLoop.GetSensitiveFunctions();
            
            Assert.NotNull(sensitiveFunctions);
            Assert.IsAssignableFrom<System.Collections.Generic.IReadOnlyCollection<string>>(sensitiveFunctions);
        }

        [Fact]
        public void GetSensitiveFunctions_ContainsDefaultSensitiveFunctions()
        {
            var sensitiveFunctions = HumanInTheLoop.GetSensitiveFunctions();
            
            Assert.Contains("SendEmail", sensitiveFunctions, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("WriteSharePointFile", sensitiveFunctions, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("SendTeamsMessage", sensitiveFunctions, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("UploadToOneDrive", sensitiveFunctions, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("CreatePlannerTask", sensitiveFunctions, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void AddSensitiveFunction_AddsNewFunction()
        {
            var testFunctionName = "TestFunction_" + Guid.NewGuid().ToString();
            
            HumanInTheLoop.AddSensitiveFunction(testFunctionName);
            var sensitiveFunctions = HumanInTheLoop.GetSensitiveFunctions();
            
            Assert.Contains(testFunctionName, sensitiveFunctions, StringComparer.OrdinalIgnoreCase);
            
            // Cleanup
            HumanInTheLoop.RemoveSensitiveFunction(testFunctionName);
        }

        [Fact]
        public void RemoveSensitiveFunction_RemovesFunction()
        {
            var testFunctionName = "TestFunctionToRemove_" + Guid.NewGuid().ToString();
            
            HumanInTheLoop.AddSensitiveFunction(testFunctionName);
            Assert.Contains(testFunctionName, HumanInTheLoop.GetSensitiveFunctions(), StringComparer.OrdinalIgnoreCase);
            
            HumanInTheLoop.RemoveSensitiveFunction(testFunctionName);
            Assert.DoesNotContain(testFunctionName, HumanInTheLoop.GetSensitiveFunctions(), StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void HumanInTheLoop_HasExpectedPublicMethods()
        {
            var expectedMethods = new[]
            {
                "ConfirmSensitiveOperationAsync",
                "AddSensitiveFunction",
                "RemoveSensitiveFunction",
                "GetSensitiveFunctions"
            };

            var publicMethods = typeof(HumanInTheLoop)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.DeclaringType == typeof(HumanInTheLoop))
                .Select(m => m.Name)
                .ToList();

            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, publicMethods);
            }
        }
    }
}
