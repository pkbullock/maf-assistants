using Xunit;
using MAF.Assistants.Services;
using System.Linq;

namespace MAF.Tests.Services
{
    public class PlannerServiceTests
    {
        [Fact]
        public void PlannerService_InheritsFromGraphService()
        {
            var baseType = typeof(PlannerService).BaseType;
            Assert.NotNull(baseType);
            Assert.Equal(typeof(GraphService), baseType);
        }

        [Fact]
        public void GetPlansAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(PlannerService).GetMethod("GetPlansAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal("userId", parameters[0].Name);
            Assert.Equal("maxItems", parameters[1].Name);
        }

        [Fact]
        public void GetTasksAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(PlannerService).GetMethod("GetTasksAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("planId", parameters[0].Name);
        }

        [Fact]
        public void GetUserTasksAsync_HasCorrectSignature()
        {
            var methodInfo = typeof(PlannerService).GetMethod("GetUserTasksAsync");
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.ReturnType.IsGenericType);
            
            var parameters = methodInfo.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("userId", parameters[0].Name);
        }

        [Fact]
        public void PlannerService_HasExpectedPublicMethods()
        {
            var expectedMethods = new[] { "GetPlansAsync", "GetTasksAsync", "GetUserTasksAsync" };
            var publicMethods = typeof(PlannerService)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.DeclaringType == typeof(PlannerService))
                .Select(m => m.Name)
                .ToList();

            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, publicMethods);
            }
        }
    }
}
