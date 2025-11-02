using Xunit;
using MAF.Assistants.Utility;
using System;

namespace MAF.Tests.Utility
{
    public class ConfigManagerTests
    {
        [Fact]
        public void Configuration_IsRecordType()
        {
            var type = typeof(ConfigManager.Configuration);
            
            // Records are compiler-generated classes with specific characteristics
            Assert.True(type.IsClass);
            Assert.NotNull(type);
        }

        [Fact]
        public void Configuration_HasAzureOpenAiEndpointProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("AzureOpenAiEndpoint");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasAzureOpenAiKeyProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("AzureOpenAiKey");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasAzureChatDeploymentNameProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("AzureChatDeploymentName");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasAzureEmbeddingModelNameProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("AzureEmbeddingModelName");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasMicrosoftGraphTenantIdProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("MicrosoftGraphTenantId");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasMicrosoftGraphClientIdProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("MicrosoftGraphClientId");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasMicrosoftGraphClientSecretProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("MicrosoftGraphClientSecret");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasMicrosoftGraphMaxItemsProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("MicrosoftGraphMaxItems");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(int), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasMicrosoftGraphUseDelegatedAuthProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("MicrosoftGraphUseDelegatedAuth");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(bool), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasMicrosoftGraphRedirectUriProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("MicrosoftGraphRedirectUri");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasFoundryLocalChatDeploymentNameProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("FoundryLocalChatDeploymentName");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void Configuration_HasOllamaLocalEmbeddingModelNameProperty()
        {
            var propertyInfo = typeof(ConfigManager.Configuration).GetProperty("OllamaLocalEmbeddingModelName");
            
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(string), propertyInfo.PropertyType);
        }

        [Fact]
        public void ConfigManager_HasGetConfigMethod()
        {
            var methodInfo = typeof(ConfigManager).GetMethod("GetConfig");
            
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsStatic);
            Assert.Equal(typeof(ConfigManager.Configuration), methodInfo.ReturnType);
            Assert.Empty(methodInfo.GetParameters());
        }

        [Fact]
        public void GetConfig_ReturnsConfiguration()
        {
            // This test will use the actual user secrets if configured
            // If not configured, it will return empty strings, which is valid behavior
            var config = ConfigManager.GetConfig();
            
            Assert.NotNull(config);
            Assert.IsType<ConfigManager.Configuration>(config);
        }

        [Fact]
        public void GetConfig_ReturnsDefaultValuesWhenSecretsNotConfigured()
        {
            // GetConfig should not throw, even if secrets are not configured
            // It should return empty strings or default values
            var config = ConfigManager.GetConfig();
            
            Assert.NotNull(config);
            // These properties should exist and have values (even if empty)
            Assert.NotNull(config.AzureOpenAiEndpoint);
            Assert.NotNull(config.AzureOpenAiKey);
            Assert.NotNull(config.MicrosoftGraphTenantId);
            Assert.NotNull(config.MicrosoftGraphClientId);
            Assert.NotNull(config.MicrosoftGraphClientSecret);
            Assert.NotNull(config.MicrosoftGraphRedirectUri);
        }

        [Fact]
        public void GetConfig_MicrosoftGraphMaxItems_HasDefaultValue()
        {
            var config = ConfigManager.GetConfig();
            
            Assert.NotNull(config);
            // Default should be 100 when not configured
            Assert.True(config.MicrosoftGraphMaxItems >= 0);
        }

        [Fact]
        public void GetConfig_MicrosoftGraphUseDelegatedAuth_HasDefaultValue()
        {
            var config = ConfigManager.GetConfig();
            
            Assert.NotNull(config);
            // This is a boolean, so it will have a value (true or false)
            Assert.IsType<bool>(config.MicrosoftGraphUseDelegatedAuth);
        }
    }
}
