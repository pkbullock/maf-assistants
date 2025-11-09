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

    /// <summary>
    /// Gets the system instructions for the agent based on its type
    /// </summary>
    public string GetInstructions()
    {
        if (!string.IsNullOrEmpty(CustomInstructions))
        {
            return CustomInstructions;
        }

        return AgentType switch
        {
            AgentType.SimpleChat => "You are a helpful and friendly AI assistant. You provide clear, accurate, and concise responses to user questions.",

            AgentType.CodeExpert => "You are an expert software developer and architect with deep knowledge of programming languages, design patterns, and best practices. You provide detailed code examples, explain complex concepts clearly, and help debug issues.",

            AgentType.DataAnalyst => "You are a data analyst expert specializing in data analysis, visualization, and statistical interpretation. You help users understand their data, create insights, and recommend appropriate analysis techniques.",

            AgentType.ContentWriter => "You are a professional content writer skilled in both creative and technical writing. You help craft engaging content, improve writing quality, and adapt tone and style to different audiences.",

            AgentType.GraphAgent => "You are an AI assistant with access to Microsoft 365 services through Microsoft Graph. You can help with emails, files, calendar, Teams, and other Microsoft 365 tasks.",

            _ => "You are a helpful AI assistant."
        };
    }

    /// <summary>
    /// Gets the default name for the agent based on its type
    /// </summary>
    public string GetDefaultName()
    {
        return AgentType switch
        {
            AgentType.SimpleChat => "General Assistant",
            AgentType.HumourWriter => "Humour Writer",

            //AgentType.CodeExpert => "Code Expert",
            //AgentType.DataAnalyst => "Data Analyst",
            //AgentType.ContentWriter => "Content Writer",
            //AgentType.GraphAgent => "Graph Agent",
            _ => "AI Assistant"
        };
    }
}
