using System.ComponentModel;

namespace MAF.Assistants.Models;

/// <summary>
/// Defines the available agent types that can be created by the factory
/// </summary>
public enum AgentType
{
    /// <summary>
    /// Simple chat agent for general conversations
    /// </summary>
    [Description("Simple Chat Agent - General purpose conversational AI")]
    SimpleChat = 0,

    /// <summary>
    /// Represents a writer that generates jokes and comedic one-liners.
    /// </summary>
    [Description("Humour Writer - Creates jokes and comedy one liners.")]
    HumourWriter = 1,
    
    /// <summary>
    /// Code expert agent specialized in programming and software development
    /// </summary>
    [Description("Code Expert - Specialized in programming and software development")]
    CodeExpert = 2,

    /// <summary>
    /// Data analyst agent for data analysis and visualization
    /// </summary>
    [Description("Data Analyst - Expert in data analysis and visualization")]
    DataAnalyst = 3,

    /// <summary>
    /// Content writer agent for creative and technical writing
    /// </summary>
    [Description("Content Writer - Specialized in creative and technical writing")]
    ContentWriter = 4,

    /// <summary>
    /// Microsoft Graph agent with access to Microsoft 365 services
    /// </summary>
    [Description("Graph Agent - Integrated with Microsoft 365 services")]
    GraphAgent = 5,

    [Description("Social Media Agent - Generate Social Media Posts")]
    SocialMediaAgent = 6
}
