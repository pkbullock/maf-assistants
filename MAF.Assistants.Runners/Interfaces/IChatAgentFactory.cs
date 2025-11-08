using MAF.Assistants.Models;
using Microsoft.Agents.AI;

namespace MAF.Assistants.Interfaces;

/// <summary>
/// Factory interface for creating AI chat agents
/// </summary>
public interface IChatAgentFactory
{
    /// <summary>
    /// Creates a configured AI chat agent with default settings
    /// </summary>
    /// <param name="isCloudMode">Whether to use cloud-based (Azure) or local model</param>
    /// <returns>An initialized AI agent</returns>
    AIAgent CreateChatAgent(bool isCloudMode = true);
    
    /// <summary>
    /// Creates a configured AI chat agent based on the provided configuration
    /// </summary>
    /// <param name="configuration">Agent configuration including type, instructions, and settings</param>
    /// <returns>An initialized AI agent</returns>
    AIAgent CreateChatAgent(AgentConfiguration configuration);
    
    /// <summary>
    /// Creates a configured AI chat agent of a specific type
    /// </summary>
    /// <param name="agentType">The type of agent to create</param>
    /// <param name="isCloudMode">Whether to use cloud-based (Azure) or local model</param>
    /// <returns>An initialized AI agent</returns>
    AIAgent CreateChatAgent(AgentType agentType, bool isCloudMode = true);
}
