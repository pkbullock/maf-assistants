namespace MAF.Assistants.Models;

/// <summary>
/// Configuration for an agent including its type, instructions, and metadata
/// </summary>
public class AgentConfiguration : BaseAgentConfiguration
{
    /// <summary>
    /// The type of agent to create
    /// </summary>
    public AgentType AgentType { get; set; } = AgentType.SimpleChat;

    /// <summary>
    /// Custom instructions for the agent (overrides default instructions for the agent type)
    /// </summary>
    public string? CustomInstructions { get; set; }

    /// <summary>
    /// Name of the agent
    /// </summary>
    public string AgentName { get; set; } = "AI Assistant";

    /// <summary>
    /// Whether to use cloud mode (Azure) or local model
    /// </summary>
    public bool IsCloudMode { get; set; } = true;

   
}
