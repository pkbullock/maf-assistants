namespace MAF.UI.Models;

/// <summary>
/// Represents a starter prompt suggestion in the UI
/// </summary>
public class StarterPrompt
{
    /// <summary>
    /// The title or short description of the prompt
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The actual prompt text to be sent when clicked
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    public StarterPrompt(string title, string prompt)
    {
        Title = title;
        Prompt = prompt;
    }

    public StarterPrompt() { }
}
