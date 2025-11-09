using Azure.AI.OpenAI;
using Microsoft.AI.Foundry.Local;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Utility
{
    public static class Clients
    {
        /// <summary>
        /// Creates and returns a new instance of the <see cref="AzureOpenAIClient"/> configured with the Azure OpenAI
        /// endpoint and API key.
        /// </summary>
        /// <remarks>The method retrieves the Azure OpenAI endpoint and API key from the application's
        /// configuration settings. Ensure that the configuration contains valid values for the endpoint and API key
        /// before calling this method.</remarks>
        /// <returns>A new instance of the <see cref="AzureOpenAIClient"/> initialized with the specified endpoint and API key.</returns>
        public static AzureOpenAIClient Azure()
        {

            AzureOpenAIClient client = new AzureOpenAIClient(new Uri(ConfigManager.GetConfig().AzureOpenAiEndpoint),
                new ApiKeyCredential(ConfigManager.GetConfig().AzureOpenAiKey));
            
            return client;
        }

        /// <summary>
        /// Retrieves a configured <see cref="ChatClient"/> instance for interacting with the Azure OpenAI service.
        /// </summary>
        /// <remarks>This method initializes a <see cref="ChatClient"/> using the Azure OpenAI deployment
        /// name specified in the application's configuration. Ensure that the Azure OpenAI service is properly
        /// configured and the deployment name is valid.</remarks>
        /// <returns>A <see cref="ChatClient"/> instance connected to the specified Azure OpenAI deployment.</returns>
        public static ChatClient GetAzureChat()
        {
            AzureOpenAIClient client = Azure();
            ChatClient chatClient = client.GetChatClient(ConfigManager.GetConfig().AzureChatDeploymentName);
            return chatClient;
        }

        /// <summary>
        /// Creates and initializes an <see cref="OpenAIClient"/> instance configured to interact with a locally hosted
        /// Foundry model.
        /// </summary>
        /// <remarks>This method starts a local Foundry server, retrieves the model information, and
        /// configures an <see cref="OpenAIClient"/>  to communicate with the server. The Foundry model is identified
        /// using the deployment name specified in the application's configuration. <para> Note: The local Foundry
        /// integration currently has limitations with the <see cref="OpenAIClient"/>, such as the inability to
        /// configure  maximum token limits or temperature settings. </para></remarks>
        /// <returns>An <see cref="OpenAIClient"/> instance configured to interact with the locally hosted Foundry model.</returns>
        public static OpenAIClient LocalFoundry(bool requiresTools = false)
        {
            // Placeholder for starting local process logic
            WriteOut.Msg("Starting local process...");

            var modelId = ConfigManager.GetConfig().FoundryLocalChatDeploymentName;

            // Start the local foundry server and get the model info.
            var manager = FoundryLocalManager.StartModelAsync(aliasOrModelId: modelId).GetAwaiter().GetResult();

            WriteOut.Divider();

            WriteOut.Msg($"Endpoint: {manager.Endpoint}");
            WriteOut.Msg($"Is Service Running: {manager.IsServiceRunning}");

            var model = manager.GetModelInfoAsync(aliasOrModelId: modelId).GetAwaiter().GetResult();

            WriteOut.Divider();
            WriteOut.Msg($"Model Provider Type: {model?.ProviderType.ToString()}");
            WriteOut.Msg("Model Type: " + model?.ModelType.ToString());
            WriteOut.Msg("Model ID: " + model?.ModelId);
            WriteOut.Msg("Model Alias: " + model?.Alias);
            WriteOut.Msg("Model Supports Tools: " + model?.SupportsToolCalling.ToString());
            
            if(requiresTools && !(model?.SupportsToolCalling ?? false))
            {
                WriteOut.MsgGrey("!Warning - The selected local Foundry model does not support tool calling, which is required.");
            }

            WriteOut.Divider();

            ApiKeyCredential key = new ApiKeyCredential(manager.ApiKey);

            // There is currently a limitiation with the local foundry and the OpenAIClient, in
            // that there are no options to set to max tokens or temperature settings.
            OpenAIClient client = new OpenAIClient(key, new OpenAIClientOptions()
            {
                Endpoint = manager.Endpoint
            });

            
            return client;
        }

        /// <summary>
        /// Creates and returns a chat client configured to interact with the local Foundry chat deployment.
        /// </summary>
        /// <remarks>This method retrieves the local Foundry client and uses the deployment name specified
        /// in the  configuration to initialize the chat client. Ensure that the configuration contains a valid 
        /// deployment name for the local Foundry chat service.</remarks>
        /// <returns>A <see cref="ChatClient"/> instance connected to the local Foundry chat deployment.</returns>
        public static ChatClient GetLocalFoundryChat(bool requiresTools = false)
        {
            var client = LocalFoundry(requiresTools);
            var modelId = ConfigManager.GetConfig().FoundryLocalChatDeploymentName;
            ChatClient chatClient = client.GetChatClient(modelId);
            return chatClient;
        }

        public static void BlockLocalFoundryChatNotSupported(ChatClient client)
        {
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (client.Model == ConfigManager.GetConfig().FoundryLocalChatDeploymentName)
            {
                throw new InvalidOperationException("The provided ChatClient is not supported for the local Foundry chat deployment.");
            }
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }
        

        public static void OpenAI()
        {
            throw new NotImplementedException();
        }

        public static void Ollama() {

            throw new NotImplementedException();
        }

        public class RunOptions
        {
            //, RunOptions? options = null
            public bool RequiresTools { get; init; }

            public RunOptions(bool requiresTools = true)
            {
                RequiresTools = requiresTools;
            }

        }
    }
}
