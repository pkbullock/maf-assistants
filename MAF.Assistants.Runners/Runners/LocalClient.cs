using Azure.AI.OpenAI;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Utility;
using Microsoft.AI.Foundry.Local;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Runners
{
    internal class LocalClient : ProcessRunners
    {

        // This is a class that will handle running local processes of models

        public LocalClient() { }


        /// <summary>
        /// Entry point to start a local process agents
        /// </summary>
        public static Task Start<T>() where T : RunModel, new()
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
            WriteOut.Divider();

            ApiKeyCredential key = new ApiKeyCredential(manager.ApiKey);

            // There is currently a limitiation with the local foundry and the OpenAIClient, in
            // that there are no options to set to max tokens or temperature settings.
            OpenAIClient client = new OpenAIClient(key, new OpenAIClientOptions()
            {
                Endpoint = manager.Endpoint
            });

            
            ChatClient chatClient = client.GetChatClient(modelId);

            var modelInstance = new T();
            return modelInstance.StartAsync(chatClient);
        }

    }
}
