using MAF.UI.Models;
using System.Text.Json;

namespace MAF.UI.Services;

/// <summary>
/// Service for persisting application settings to local storage
/// </summary>
public class SettingsStorageService
{
    private readonly string _settingsDirectory;
    private readonly string _settingsFilePath;

    public SettingsStorageService()
    {
        _settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MAF.UI"
        );
        _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");
    }

    /// <summary>
    /// Saves settings to local storage
    /// </summary>
    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            // Ensure directory exists
            if (!Directory.Exists(_settingsDirectory))
            {
                Directory.CreateDirectory(_settingsDirectory);
            }

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Loads settings from local storage
    /// </summary>
    public async Task<AppSettings?> LoadSettingsAsync()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(_settingsFilePath);
            return JsonSerializer.Deserialize<AppSettings>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets the settings file path for display purposes
    /// </summary>
    public string GetSettingsPath() => _settingsFilePath;
}
