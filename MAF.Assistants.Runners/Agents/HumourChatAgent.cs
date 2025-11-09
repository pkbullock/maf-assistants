using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using System;

namespace MAF.Assistants.Agents
{
    /// <summary>
    /// Humour chat agent specialized in telling jokes and funny responses
    /// </summary>
    public class HumourChatAgent : IChatAgent
    {
        /// <summary>
        /// Default configuration for the humour chat agent
        /// </summary>
        public static BaseAgentConfiguration DefaultConfiguration => new BaseAgentConfiguration
        {
            AgentType = AgentType.HumourWriter,
            Instructions = "You are good at telling jokes. You provide humorous, witty responses while being helpful. You can tell jokes on demand and add humor to your explanations.",
            AgentName = "JokeAI",
            Description = "A funny AI assistant that tells jokes and provides humorous responses",
            IconEmoji = "😄",
            DefaultIsCloudMode = true
        };

        /// <summary>
        /// Creates a chat agent with default configuration and specified cloud mode
        /// </summary>
        public AIAgent CreateChatAgent(bool isCloudMode = true)
        {
            var config = new AgentConfiguration
            {
                AgentType = DefaultConfiguration.AgentType,
                CustomInstructions = DefaultConfiguration.Instructions,
                AgentName = DefaultConfiguration.AgentName,
                IsCloudMode = isCloudMode
            };

            return CreateChatAgent(config);
        }

        /// <summary>
        /// Creates a chat agent with the specified configuration
        /// </summary>
        public AIAgent CreateChatAgent(AgentConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            string instructions = string.IsNullOrEmpty(configuration.CustomInstructions)
                ? DefaultConfiguration.Instructions
                : configuration.CustomInstructions;

            string agentName = string.IsNullOrEmpty(configuration.AgentName)
                ? DefaultConfiguration.AgentName
                : configuration.AgentName;

            ChatClient client = configuration.IsCloudMode 
                ? Clients.GetAzureChat() 
                : Clients.GetLocalFoundryChat();

            ChatClientAgent agent = client.CreateAIAgent(instructions: instructions, name: agentName);

            return agent;
        }
    }
}
