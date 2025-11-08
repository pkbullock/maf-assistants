namespace MAF.UI.Models;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public List<string>? AttachedFiles { get; set; }
}
