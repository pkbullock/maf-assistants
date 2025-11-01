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
            string OllamaLocalEmbeddingModelName,
            
            string MicrosoftGraphTenantId,
            string MicrosoftGraphClientId,
            string MicrosoftGraphClientSecret,
            int MicrosoftGraphMaxItems
         );

        /// <summary>
        /// Gets the configuration settings
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Usage: ConfigManager.GetConfig().AzureOpenAiEndpoint
        /// </remarks>
        public static Configuration GetConfig()
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddUserSecrets<ConfigManager>().Build();
            
            string azureOpenAiEndpoint = configurationRoot["AzureOpenAiEndpoint"] ?? string.Empty;
            string azureOpenAiKey = configurationRoot["AzureOpenAiKey"] ?? string.Empty;
            string azureChatDeploymentName = configurationRoot["AzureChatDeploymentName"] ?? string.Empty;
            string azureEmbeddingModelName = configurationRoot["AzureEmbeddingModelName"] ?? string.Empty;

            string foundryLocalChatDeploymentName = configurationRoot["FoundryLocalChatDeploymentName"] ?? string.Empty;
            string ollamaLocalEmbeddingModelName = configurationRoot["OllamaLocalEmbeddingModelName"] ?? string.Empty;

            string microsoftGraphTenantId = configurationRoot["MicrosoftGraphTenantId"] ?? string.Empty;
            string microsoftGraphClientId = configurationRoot["MicrosoftGraphClientId"] ?? string.Empty;
            string microsoftGraphClientSecret = configurationRoot["MicrosoftGraphClientSecret"] ?? string.Empty;
            int microsoftGraphMaxItems = int.TryParse(configurationRoot["MicrosoftGraphMaxItems"], out int maxItems) ? maxItems : 100;

            return new Configuration(
                azureOpenAiEndpoint,
                azureOpenAiKey,
                azureChatDeploymentName,
                azureEmbeddingModelName,

                foundryLocalChatDeploymentName,
                ollamaLocalEmbeddingModelName,
                
                microsoftGraphTenantId,
                microsoftGraphClientId,
                microsoftGraphClientSecret,
                microsoftGraphMaxItems);
        }
    }
}
