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
    /// Content writer agent specialized in creative and technical writing
    /// </summary>
    public class ContentWriterAgent : IChatAgent
    {
        /// <summary>
      /// Default configuration for the content writer agent
        /// </summary>
    public static BaseAgentConfiguration DefaultConfiguration => new BaseAgentConfiguration
        {
       AgentType = AgentType.ContentWriter,
   Instructions = "You are a professional content writer skilled in both creative and technical writing. You help craft engaging content, improve writing quality, and adapt tone and style to different audiences. You can assist with blog posts, articles, documentation, marketing copy, and creative writing. You provide constructive feedback and suggestions to enhance clarity and impact.",
     AgentName = "Content Writer",
       Description = "Specialized in creative and technical writing",
        IconEmoji = "??",
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
