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
    /// Code expert agent specialized in programming and software development
    /// </summary>
    public class CodeExpertAgent : IChatAgent
    {
        /// <summary>
        /// Default configuration for the code expert agent
        /// </summary>
        public static BaseAgentConfiguration DefaultConfiguration => new BaseAgentConfiguration
        {
            AgentType = AgentType.CodeExpert,
            Instructions = "You are an expert software developer and architect with deep knowledge of programming languages, design patterns, and best practices. You provide detailed code examples, explain complex concepts clearly, and help debug issues. You stay current with modern development practices and can advise on architecture decisions.",
            AgentName = "Code Expert",
            Description = "Specialized in programming and software development",
            IconEmoji = "💻",
            DefaultIsCloudMode = true,
            StarterPrompts = new List<StarterPrompt>
            {
                new StarterPrompt("Explain the SOLID principles", "Explain the SOLID principles in software development"),
                new StarterPrompt("How to optimize database queries?", "How can I optimize my database queries for better performance?"),
                new StarterPrompt("Review this code snippet", "Can you review this code snippet and suggest improvements?"),
                new StarterPrompt("Best practices for unit testing", "What are the best practices for writing unit tests?")
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
