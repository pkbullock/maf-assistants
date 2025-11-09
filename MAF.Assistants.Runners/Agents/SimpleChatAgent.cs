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
    /// Simple chat agent for general purpose conversations
    /// </summary>
    public class SimpleChatAgent : IChatAgent
    {
        /// <summary>
        /// Default configuration for the simple chat agent
        /// </summary>
        public static BaseAgentConfiguration DefaultConfiguration => new BaseAgentConfiguration
        {
            //TODO: Add Start Prompts for the UI
            AgentType = AgentType.SimpleChat,
            Instructions = "You are a helpful and friendly AI assistant. You provide clear, accurate, and concise responses to user questions.",
            AgentName = "General Assistant",
            Description = "General purpose conversational AI",
            IconEmoji = "🤖",
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

            //TODO: Custom Instructions should be in addtion of, rather than an override to ensure that base functionality isnt overriden.
            string instructions = string.IsNullOrEmpty(configuration.CustomInstructions)
                ? DefaultConfiguration.Instructions
                : configuration.CustomInstructions;

            //TODO: The agent name shouldnt be overridable 
            string agentName = string.IsNullOrEmpty(configuration.AgentName)
                ? DefaultConfiguration.AgentName
                : configuration.AgentName;

            //TODO: Expand to introduce a block, ie. what if local mode isnt available for this type of agent
            ChatClient client = configuration.IsCloudMode
                ? Clients.GetAzureChat()
                : Clients.GetLocalFoundryChat();

            ChatClientAgent agent = client.CreateAIAgent(instructions: instructions, name: agentName);

            return agent;
        }
    }
}
