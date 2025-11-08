namespace MAF.UI.Models;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public List<string>? AttachedFiles { get; set; }
    
    /// <summary>
    /// Optional Adaptive Card JSON content
    /// </summary>
    public string? AdaptiveCardJson { get; set; }
    
    /// <summary>
    /// Indicates whether this message contains an Adaptive Card
    /// </summary>
    public bool HasAdaptiveCard => !string.IsNullOrEmpty(AdaptiveCardJson);
}
