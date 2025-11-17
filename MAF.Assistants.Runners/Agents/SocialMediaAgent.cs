using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System;

namespace MAF.Assistants.Agents
{
    public class SocialMediaAgent : IChatAgent
    {
        public static BaseAgentConfiguration DefaultConfiguration => new BaseAgentConfiguration
        {
            AgentType = AgentType.SocialMediaAgent,
            Instructions = "You are a social media authoring agent. Yuo helpp create social media posts, captions, and content strategies tailored to different platforms and audiences. You provide insights on trending topics, optimal posting times, and engagement techniques to maximize reach and interaction.",
            AgentName = "Social Media Agent",
            Description = "Specialising in generating Social Media Posts",
            IconEmoji = "💬",
            DefaultIsCloudMode = true,
            StarterPrompts = {
                new StarterPrompt("Create BlueSky post", "Help me create an engaging BlueSky post about healthy living."),
                new StarterPrompt("Generate X thread", "Generate a Twitter thread discussing the latest trends in technology."),
                new StarterPrompt("Craft LinkedIn article", "Assist me in crafting a LinkedIn article on professional development tips."),
                new StarterPrompt("Suggest posting times", "What are the best times to post on social media for maximum engagement?")
            }
        };

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

        public AIAgent CreateChatAgent(AgentConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            string instructions = string.IsNullOrEmpty(configuration.CustomInstructions)
                    ? DefaultConfiguration.Instructions : configuration.CustomInstructions;

            string agentName = string.IsNullOrEmpty(configuration.AgentName)
                   ? DefaultConfiguration.AgentName : configuration.AgentName;

            ChatClient client = configuration.IsCloudMode
                    ? Clients.GetAzureChat() : Clients.GetLocalFoundryChat();

            ChatClientAgent agent = client.CreateAIAgent(instructions: instructions, name: agentName);

            return agent;
        }
    }
}
