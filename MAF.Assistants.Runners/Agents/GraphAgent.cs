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
    /// Graph agent with access to Microsoft 365 services
    /// </summary>
    public class GraphAgent : IChatAgent
    {
        /// <summary>
        /// Default configuration for the graph agent
        /// </summary>
        public static BaseAgentConfiguration DefaultConfiguration => new BaseAgentConfiguration
        {
            AgentType = AgentType.GraphAgent,
            Instructions = "You are an AI assistant with access to Microsoft 365 services through Microsoft Graph. You can help with emails, files, calendar, Teams, SharePoint, OneDrive, Planner, and other Microsoft 365 tasks. You have tools to read, create, and manage Microsoft 365 content. Always confirm actions that modify data before executing them.",
            AgentName = "Graph Agent",
            Description = "Integrated with Microsoft 365 services",
            IconEmoji = "📁",
            DefaultIsCloudMode = true,
            StarterPrompts = new List<StarterPrompt>
            {
                new StarterPrompt("Show my recent emails", "Show me my recent emails from today"),
                new StarterPrompt("Check my calendar", "What meetings do I have today?"),
                new StarterPrompt("List OneDrive files", "List the recent files in my OneDrive"),
                new StarterPrompt("Send an email", "Help me send an email to my team")
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

            ChatClientAgent agent = client.CreateAIAgent(instructions: instructions, name: agentName);

            return agent;
        }
    }
}
