namespace MAF.Assistants.Models;

/// <summary>
/// Represents a starter prompt suggestion that helps users begin a conversation
/// </summary>
public class StarterPrompt
{
    /// <summary>
    /// The title or short description of the prompt
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The actual prompt text to be sent when the user clicks on this starter
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new starter prompt with the specified title and prompt text
    /// </summary>
    public StarterPrompt(string title, string prompt)
    {
        Title = title;
        Prompt = prompt;
    }

    /// <summary>
    /// Parameterless constructor for serialization
    /// </summary>
    public StarterPrompt() { }
}
