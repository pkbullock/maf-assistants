using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Agents
{
    public class SimpleChatAgent
    {
        public AIAgent CreateChatAgent(bool isCloudMode = true)
        {
            string modelInstruction = "You are good at telling jokes.";
            string agentName = "JokeAI";

            ChatClient client = isCloudMode ? Clients.GetAzureChat() : Clients.GetLocalFoundryChat();

            ChatClientAgent agent = client.CreateAIAgent(instructions: modelInstruction, name: agentName);

            return agent;
        }
    }
}
