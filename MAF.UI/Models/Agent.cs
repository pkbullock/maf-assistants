namespace MAF.UI.Models;

public class Agent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Default Agent";
    public string Description { get; set; } = string.Empty;
    public string IconEmoji { get; set; } = "🤖";
    public bool IsDefault { get; set; } = false;
}
