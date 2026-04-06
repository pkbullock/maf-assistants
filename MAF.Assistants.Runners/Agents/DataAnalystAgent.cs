using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;

namespace MAF.Assistants.Agents
{
    /// <summary>
    /// Data analyst agent specialized in data analysis and visualization
    /// </summary>
    public class DataAnalystAgent : IChatAgent
    {
        /// <summary>
        /// Default configuration for the data analyst agent
        /// </summary>
        public static BaseAgentConfiguration DefaultConfiguration => new BaseAgentConfiguration
        {
            AgentType = AgentType.DataAnalyst,
            Instructions = "You are a data analyst expert specializing in data analysis, visualization, and statistical interpretation. You help users understand their data, create insights, and recommend appropriate analysis techniques. You can suggest visualization approaches, explain statistical concepts, and guide users through data-driven decision making.",
            AgentName = "Data Analyst",
            Description = "Expert in data analysis and visualization",
            IconEmoji = "📊",
            DefaultIsCloudMode = true,
            StarterPrompts = new List<StarterPrompt>
            {
                new StarterPrompt("How to visualize sales trends?", "How can I visualize sales trends over time effectively?"),
                new StarterPrompt("Explain statistical significance", "Explain statistical significance in simple terms"),
                new StarterPrompt("Best chart for comparing data", "What's the best chart type for comparing categorical data?"),
                new StarterPrompt("Analyze customer behavior patterns", "How can I analyze customer behavior patterns from data?")
            }
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

            ChatClientAgent agent = client.AsAIAgent(instructions: instructions, name: agentName);

            return agent;
        }
    }
}
