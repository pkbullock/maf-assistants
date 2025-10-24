using Microsoft.Extensions.Configuration;

namespace MAF.Assistants.Utility
{
    public class ConfigManager
    {
        public record Configuration(
            string AzureOpenAiEndpoint,
            string AzureOpenAiKey,
            string AzureChatDeploymentName,
            string AzureEmbeddingModelName,
            
            string FoundryLocalChatDeploymentName,
            string OllamaLocalEmbeddingModelName
         );

        public static Configuration GetConfig()
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddUserSecrets<ConfigManager>().Build();
            
            string azureOpenAiEndpoint = configurationRoot["AzureOpenAiEndpoint"] ?? string.Empty;
            string azureOpenAiKey = configurationRoot["AzureOpenAiKey"] ?? string.Empty;
            string azureChatDeploymentName = configurationRoot["AzureChatDeploymentName"] ?? string.Empty;
            string azureEmbeddingModelName = configurationRoot["AzureEmbeddingModelName"] ?? string.Empty;

            string foundryLocalChatDeploymentName = configurationRoot["FoundryLocalChatDeploymentName"] ?? string.Empty;
            string ollamaLocalEmbeddingModelName = configurationRoot["OllamaLocalEmbeddingModelName"] ?? string.Empty;

            return new Configuration(
                azureOpenAiEndpoint,
                azureOpenAiKey,
                azureChatDeploymentName,
                azureEmbeddingModelName,

                foundryLocalChatDeploymentName,
                ollamaLocalEmbeddingModelName);
        }
    }
}
