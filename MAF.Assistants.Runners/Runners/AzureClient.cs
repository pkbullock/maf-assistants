using Azure.AI.OpenAI;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Utility;
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
    internal class AzureClient : ProcessRunners
    {

        public AzureClient() { }

        /// <summary>
        /// Start the Azure OpenAI process runner and entry point for the application
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static Task StartChat<T>() where T : RunChatModel, new()
        {
            ChatClient chatClient = Clients.GetAzureChat();

            var model = new T();
            return model.StartAsync(chatClient);
        }
    }
}
