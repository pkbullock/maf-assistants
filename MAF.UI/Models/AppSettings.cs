using MAF.Assistants.Models;

namespace MAF.UI.Models;

public class AppSettings
{
    public bool IsMockMode { get; set; } = true;
    public bool IsCloudMode { get; set; } = true;
    public bool IsTestMode { get; set; } = true;

    /// <summary>
    /// The currently selected agent type
    /// </summary>
    public AgentType SelectedAgentType { get; set; } = AgentType.SimpleChat;
}
