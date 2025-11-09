using MAF.Assistants.Models;
using System.Collections.Generic;

namespace MAF.UI.Models;

public class Agent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Default Agent";
    public string Description { get; set; } = string.Empty;
    public string IconEmoji { get; set; } = "🤖";
    public bool IsDefault { get; set; } = false;
    
    /// <summary>
    /// The underlying agent type from MAF.Assistants
    /// </summary>
    public AgentType AgentType { get; set; } = AgentType.SimpleChat;
    
    /// <summary>
    /// Starter prompts for this agent
    /// </summary>
    public List<StarterPrompt> StarterPrompts { get; set; } = new();
}
