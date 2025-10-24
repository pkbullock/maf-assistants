using Azure.AI.OpenAI;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
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
    internal class SimpleChat: RunModel
    {
        public async Task StartAsync(ChatClient client)
        {
            WriteOut.MsgCyan("Starting Simple Chat...");
            WriteOut.MsgBlankLine();
            
            string modelInstruction = "You are good at telling jokes.";
            string agentName = "JokeAI";
            string prompt = "Tell me a joke about a pirate.";

            AIAgent agent = client.CreateAIAgent(instructions: modelInstruction, name: agentName);

            // Simple Example - Streaming
            // TODO: Move away from console.writeline and use the WriteOut utility class
            Console.WriteLine(await agent.RunAsync(prompt));

            await foreach (var update in agent.RunStreamingAsync("Tell me a joke about a pirate."))
            {
                Console.Write(update);
            }

        }

    }
}
