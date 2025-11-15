using MAF.Assistants.Agents;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using Microsoft.Agents.AI;
using System;
using System.Collections.Generic;

namespace MAF.Assistants.Factories
{
    /// <summary>
    /// Registry that maps AgentType enum values to concrete agent implementations
    /// </summary>
    public static class AgentRegistry
    {
        // Use delegates instead of IChatAgent to avoid static abstract interface issues
        private static readonly Dictionary<AgentType, Func<bool, AIAgent>> _simpleAgentFactories = new()
        {
            { AgentType.SimpleChat, (isCloudMode) => new SimpleChatAgent().CreateChatAgent(isCloudMode) },
            { AgentType.HumourWriter, (isCloudMode) => new HumourChatAgent().CreateChatAgent(isCloudMode) },
            { AgentType.CodeExpert, (isCloudMode) => new CodeExpertAgent().CreateChatAgent(isCloudMode) },
            { AgentType.DataAnalyst, (isCloudMode) => new DataAnalystAgent().CreateChatAgent(isCloudMode) },
            { AgentType.ContentWriter, (isCloudMode) => new ContentWriterAgent().CreateChatAgent(isCloudMode) },
            { AgentType.GraphAgent, (isCloudMode) => new GraphAgent().CreateChatAgent(isCloudMode) },
            { AgentType.SocialMediaAgent, (isCloudMode) => new SocialMediaAgent().CreateChatAgent(isCloudMode) }
        };

        private static readonly Dictionary<AgentType, Func<AgentConfiguration, AIAgent>> _configAgentFactories = new()
        {
            { AgentType.SimpleChat, (config) => new SimpleChatAgent().CreateChatAgent(config) },
            { AgentType.HumourWriter, (config) => new HumourChatAgent().CreateChatAgent(config) },
            { AgentType.CodeExpert, (config) => new CodeExpertAgent().CreateChatAgent(config) },
            { AgentType.DataAnalyst, (config) => new DataAnalystAgent().CreateChatAgent(config) },
            { AgentType.ContentWriter, (config) => new ContentWriterAgent().CreateChatAgent(config) },
            { AgentType.GraphAgent, (config) => new GraphAgent().CreateChatAgent(config) },
            { AgentType.SocialMediaAgent, (config) => new SocialMediaAgent().CreateChatAgent(config)   }
        };

        /// <summary>
        /// Creates an AI agent for the specified agent type with cloud mode
        /// </summary>
        /// <param name="agentType">The type of agent to create</param>
        /// <param name="isCloudMode">Whether to use cloud or local deployment</param>
        /// <returns>An initialized AI agent</returns>
        /// <exception cref="ArgumentException">Thrown when agent type is not registered</exception>
        public static AIAgent CreateAgent(AgentType agentType, bool isCloudMode = true)
        {
            if (_simpleAgentFactories.TryGetValue(agentType, out var factory))
            {
                return factory(isCloudMode);
            }

            throw new ArgumentException($"No agent registered for type: {agentType}", nameof(agentType));
        }

        /// <summary>
        /// Creates an AI agent for the specified configuration
        /// </summary>
        /// <param name="configuration">Agent configuration</param>
        /// <returns>An initialized AI agent</returns>
        /// <exception cref="ArgumentException">Thrown when agent type is not registered</exception>
        public static AIAgent CreateAgent(AgentConfiguration configuration)
        {
            if (_configAgentFactories.TryGetValue(configuration.AgentType, out var factory))
            {
                return factory(configuration);
            }

            throw new ArgumentException($"No agent registered for type: {configuration.AgentType}", nameof(configuration));
        }

        /// <summary>
        /// Gets the default configuration for the specified agent type
        /// </summary>
        /// <param name="agentType">The type of agent</param>
        /// <returns>Base agent configuration</returns>
        public static BaseAgentConfiguration GetDefaultConfiguration(AgentType agentType)
        {
            return agentType switch
            {
                AgentType.SimpleChat => SimpleChatAgent.DefaultConfiguration,
                AgentType.HumourWriter => HumourChatAgent.DefaultConfiguration,
                AgentType.CodeExpert => CodeExpertAgent.DefaultConfiguration,
                AgentType.DataAnalyst => DataAnalystAgent.DefaultConfiguration,
                AgentType.ContentWriter => ContentWriterAgent.DefaultConfiguration,
                AgentType.GraphAgent => GraphAgent.DefaultConfiguration,
                AgentType.SocialMediaAgent => SocialMediaAgent.DefaultConfiguration,
                _ => throw new ArgumentException($"No configuration registered for type: {agentType}", nameof(agentType))
            };
        }

        /// <summary>
        /// Checks if an agent type is registered
        /// </summary>
        /// <param name="agentType">The type of agent to check</param>
        /// <returns>True if registered, false otherwise</returns>
        public static bool IsRegistered(AgentType agentType)
        {
            return _simpleAgentFactories.ContainsKey(agentType);
        }

        /// <summary>
        /// Gets all registered agent types
        /// </summary>
        /// <returns>Collection of registered agent types</returns>
        public static IEnumerable<AgentType> GetRegisteredTypes()
        {
            return _simpleAgentFactories.Keys;
        }
    }
}
