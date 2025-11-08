using MAF.UI.Models;
using System.Text.Json;

namespace MAF.UI.Services;

public class ChatStorageService
{
    private readonly string _storagePath;
    private readonly string _sessionsFile;

    public ChatStorageService()
    {
        // Store in user's local app data directory
        _storagePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MAF.UI",
            "ChatHistory"
        );
        _sessionsFile = Path.Combine(_storagePath, "sessions.json");

        // Ensure directory exists
        Directory.CreateDirectory(_storagePath);
    }

    public async Task SaveSessionsAsync(List<ChatSession> sessions)
    {
        try
        {
            var json = JsonSerializer.Serialize(sessions, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(_sessionsFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving sessions: {ex.Message}");
        }
    }

    public async Task<List<ChatSession>> LoadSessionsAsync()
    {
        try
        {
            if (File.Exists(_sessionsFile))
            {
                var json = await File.ReadAllTextAsync(_sessionsFile);
                return JsonSerializer.Deserialize<List<ChatSession>>(json) ?? new List<ChatSession>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading sessions: {ex.Message}");
        }

        return new List<ChatSession>();
    }

    public async Task ExportChatAsync(ChatSession session, string exportPath)
    {
        try
        {
            var content = new System.Text.StringBuilder();
            content.AppendLine($"# {session.Title}");
            content.AppendLine($"Created: {session.CreatedAt:yyyy-MM-dd HH:mm}");
            content.AppendLine($"Last Updated: {session.LastMessageAt:yyyy-MM-dd HH:mm}");
            content.AppendLine();
            content.AppendLine("---");
            content.AppendLine();

            foreach (var message in session.Messages)
            {
                var sender = message.IsUser ? "**You**" : "**AI Assistant**";
                content.AppendLine($"### {sender} ({message.Timestamp:HH:mm})");
                content.AppendLine();
                content.AppendLine(message.Content);
                
                if (message.AttachedFiles != null && message.AttachedFiles.Any())
                {
                    content.AppendLine();
                    content.AppendLine("*Attached files:*");
                    foreach (var file in message.AttachedFiles)
                    {
                        content.AppendLine($"- {file}");
                    }
                }
                
                content.AppendLine();
                content.AppendLine("---");
                content.AppendLine();
            }

            await File.WriteAllTextAsync(exportPath, content.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting chat: {ex.Message}");
            throw;
        }
    }

    public string GetStoragePath() => _storagePath;
}
