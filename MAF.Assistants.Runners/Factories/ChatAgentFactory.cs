using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using Microsoft.Agents.AI;
using System;

namespace MAF.Assistants.Factories;

/// <summary>
/// Factory for creating AI chat agents with configurable cloud or local deployment
/// </summary>
public class ChatAgentFactory : IChatAgentFactory
{
    /// <summary>
    /// Creates a new instance of ChatAgentFactory
    /// </summary>
    public ChatAgentFactory()
    {
    }

    /// <summary>
    /// Creates a configured AI chat agent with default settings (SimpleChat type)
    /// </summary>
    /// <param name="isCloudMode">Whether to use cloud-based (Azure) or local model</param>
    /// <returns>An initialized AI agent</returns>
    public AIAgent CreateChatAgent(bool isCloudMode = true)
    {
        // Default to SimpleChat when no type is specified
      return CreateChatAgent(AgentType.SimpleChat, isCloudMode);
    }

    /// <summary>
    /// Creates a configured AI chat agent of a specific type using the pre-defined agent class
    /// </summary>
    /// <param name="agentType">The type of agent to create</param>
    /// <param name="isCloudMode">Whether to use cloud-based (Azure) or local model</param>
    /// <returns>An initialized AI agent</returns>
    public AIAgent CreateChatAgent(AgentType agentType, bool isCloudMode = true)
    {
        // Use the registry to create the agent with the specific type
        return AgentRegistry.CreateAgent(agentType, isCloudMode);
    }

    /// <summary>
    /// Creates a configured AI chat agent based on the provided configuration using the pre-defined agent class
    /// </summary>
    /// <param name="configuration">Agent configuration including type, instructions, and settings</param>
    /// <returns>An initialized AI agent</returns>
    public AIAgent CreateChatAgent(AgentConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Use the registry to create the agent with the configuration
        return AgentRegistry.CreateAgent(configuration);
    }
}
