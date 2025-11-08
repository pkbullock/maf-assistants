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
    /// Code expert agent specialized in programming and software development
    /// </summary>
    [Description("Code Expert - Specialized in programming and software development")]
CodeExpert = 1,
    
    /// <summary>
    /// Data analyst agent for data analysis and visualization
    /// </summary>
    [Description("Data Analyst - Expert in data analysis and visualization")]
    DataAnalyst = 2,
    
    /// <summary>
    /// Content writer agent for creative and technical writing
    /// </summary>
    [Description("Content Writer - Specialized in creative and technical writing")]
    ContentWriter = 3,
  
    /// <summary>
    /// Microsoft Graph agent with access to Microsoft 365 services
    /// </summary>
    [Description("Graph Agent - Integrated with Microsoft 365 services")]
    GraphAgent = 4
}
