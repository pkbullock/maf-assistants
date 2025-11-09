using MAF.Assistants.Models;

namespace MAF.UI.Models;

public class AppSettings
{
    public bool IsCloudMode { get; set; } = true;
    
    /// <summary>
    /// Test Mode enables simulated responses without backend connection.
    /// Combines the functionality of previous Mock Mode and Test Mode features.
    /// </summary>
    public bool IsTestMode { get; set; } = true;

    /// <summary>
    /// The currently selected agent type
    /// </summary>
    public AgentType SelectedAgentType { get; set; } = AgentType.SimpleChat;
}
