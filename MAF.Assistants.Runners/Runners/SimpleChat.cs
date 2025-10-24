using Azure.AI.OpenAI;
using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;

namespace MAF.Assistants.Runners
{
    internal class SimpleChat
    {

        public async Task StartAsync()
        {
            //OpenAIClient
            Console.WriteLine("Starting Simple Chat...");
            // Placeholder for Simple Chat logic

            string modelInstruction = "You are good at telling jokes.";
            string agentName = "JokeAI";
            string prompt = "Tell me a joke about a pirate.";


            AIAgent agent = new AzureOpenAIClient(new Uri(ConfigManager.GetConfig().AzureOpenAiEndpoint),
                new ApiKeyCredential(ConfigManager.GetConfig().AzureOpenAiKey))
                .GetChatClient(ConfigManager.GetConfig().AzureChatDeploymentName)
                .CreateAIAgent(instructions: modelInstruction, name: agentName);

            // Simple Example

            Console.WriteLine(await agent.RunAsync(prompt));

            await foreach (var update in agent.RunStreamingAsync("Tell me a joke about a pirate."))
            {
                Console.Write(update);
            }

        }
    }
}
