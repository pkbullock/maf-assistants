using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using System;

namespace MAF.Assistants.Factories;

/// <summary>
/// Factory for creating AI chat agents with configurable cloud or local deployment
/// </summary>
public class ChatAgentFactory : IChatAgentFactory
{
    private readonly string _modelInstruction;
    private readonly string _agentName;

    /// <summary>
    /// Creates a new instance of ChatAgentFactory with default settings
    /// </summary>
    public ChatAgentFactory()
        : this("You are a helpful AI assistant.", "AI Assistant")
    {
    }

    /// <summary>
    /// Creates a new instance of ChatAgentFactory with custom settings
    /// </summary>
    /// <param name="modelInstruction">The system instruction for the agent</param>
    /// <param name="agentName">The name of the agent</param>
    public ChatAgentFactory(string modelInstruction, string agentName)
    {
        _modelInstruction = modelInstruction ?? throw new ArgumentNullException(nameof(modelInstruction));
        _agentName = agentName ?? throw new ArgumentNullException(nameof(agentName));
    }

    /// <summary>
    /// Creates a configured AI chat agent with default settings
    /// </summary>
    /// <param name="isCloudMode">Whether to use cloud-based (Azure) or local model</param>
    /// <returns>An initialized AI agent</returns>
    public AIAgent CreateChatAgent(bool isCloudMode = true)
    {
        ChatClient client = isCloudMode
            ? Clients.GetAzureChat()
            : Clients.GetLocalFoundryChat();

        ChatClientAgent agent = client.CreateAIAgent(instructions: _modelInstruction, name: _agentName);

        return agent;
    }

    /// <summary>
    /// Creates a configured AI chat agent of a specific type
    /// </summary>
    /// <param name="agentType">The type of agent to create</param>
    /// <param name="isCloudMode">Whether to use cloud-based (Azure) or local model</param>
    /// <returns>An initialized AI agent</returns>
    public AIAgent CreateChatAgent(AgentType agentType, bool isCloudMode = true)
    {
        var config = new AgentConfiguration
        {
            AgentType = agentType,
            IsCloudMode = isCloudMode,
            AgentName = new AgentConfiguration { AgentType = agentType }.GetDefaultName()
        };

        return CreateChatAgent(config);
    }

    /// <summary>
    /// Creates a configured AI chat agent based on the provided configuration
    /// </summary>
    /// <param name="configuration">Agent configuration including type, instructions, and settings</param>
    /// <returns>An initialized AI agent</returns>
    public AIAgent CreateChatAgent(AgentConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        ChatClient client = configuration.IsCloudMode
            ? Clients.GetAzureChat()
            : Clients.GetLocalFoundryChat();

        string instructions = configuration.GetInstructions();
        string name = string.IsNullOrEmpty(configuration.AgentName)
            ? configuration.GetDefaultName()
            : configuration.AgentName;

        ChatClientAgent agent = client.CreateAIAgent(instructions: instructions, name: name);

        return agent;
    }
}
