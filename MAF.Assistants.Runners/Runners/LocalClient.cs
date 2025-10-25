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
        public static Task StartChat<T>() where T : RunChatModel, new()
        {

            var modelInstance = new T();

            ChatClient chatClient = Clients.GetLocalFoundryChat(modelInstance.RequiresTools);
                        
            return modelInstance.StartAsync(chatClient);
        }

    }
}
