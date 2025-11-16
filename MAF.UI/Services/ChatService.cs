using MAF.UI.Models;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using MAF.Assistants.Utilities;
using MAF.Assistants.Factories;
using Microsoft.Agents.AI;
using System.Linq;
using System.Threading;
using System.Text;

namespace MAF.UI.Services;

public class ChatService
{
    private readonly List<ChatSession> _sessions = new();
    private readonly List<Agent> _agents = new();
    private readonly AppSettings _settings = new();
    private ChatSession? _currentSession;
    private readonly ChatStorageService _storageService;
    private readonly SettingsStorageService _settingsStorageService;
    private readonly MarkdownService _markdownService;
    private readonly AdaptiveCardService _adaptiveCardService;
    private readonly IChatAgentFactory _chatAgentFactory;
    private AgentService? _agentService;

    public event Action? OnChange;

    // Cancellation source for the current streaming response
    private CancellationTokenSource? _currentResponseCts;

    // Indicates whether an agent response is currently streaming
    public bool IsAgentTyping { get; private set; }

    public ChatService(ChatStorageService storageService, SettingsStorageService settingsStorageService, MarkdownService markdownService, AdaptiveCardService adaptiveCardService, IChatAgentFactory chatAgentFactory)
    {
        _storageService = storageService;
        _settingsStorageService = settingsStorageService;
        _markdownService = markdownService;
        _adaptiveCardService = adaptiveCardService;
        _chatAgentFactory = chatAgentFactory;
        
        // Initialize available agents
        InitializeAgents();
  
        // Load saved sessions and settings
        _ = LoadSessionsAsync();
        _ = LoadSettingsAsync();
    }

    public AppSettings Settings => _settings;
    
    public MarkdownService Markdown => _markdownService;
    
    public AdaptiveCardService AdaptiveCard => _adaptiveCardService;

    private void InitializeAgents()
    {
        // Initialize agents based on available AgentTypes
        foreach (var agentType in AgentTypeHelper.GetAllAgentTypes())
        {
            var config = AgentRegistry.GetDefaultConfiguration(agentType);
            _agents.Add(new Agent
            {
                Id = agentType.ToString().ToLower(),
                Name = agentType.GetDisplayName(),
                Description = agentType.GetFullDescription(),
                IconEmoji = agentType.GetIcon(),
                IsDefault = agentType == AgentType.SimpleChat,
                AgentType = agentType,
                StarterPrompts = config.StarterPrompts.Select(sp => new UI.Models.StarterPrompt(sp.Title, sp.Prompt)).ToList()
            });
        }
    }

    public List<Agent> GetAvailableAgents() => _agents;

    public List<UI.Models.StarterPrompt> GetStarterPrompts()
    {
        var currentAgent = _agents.FirstOrDefault(a => a.AgentType == _settings.SelectedAgentType);
        return currentAgent?.StarterPrompts ?? new List<UI.Models.StarterPrompt>();
    }

    private async Task LoadSessionsAsync()
    {
        try
        {
            var sessions = await _storageService.LoadSessionsAsync();
            if (sessions.Any())
            {
                _sessions.Clear();
                _sessions.AddRange(sessions);
            }
            else
            {
                // Initialize with sample data for testing if no saved data
                _sessions.AddRange(MockDataProvider.CreateSampleSessions());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading sessions: {ex.Message}");
            _sessions.AddRange(MockDataProvider.CreateSampleSessions());
        }
    }

    public async Task SaveSessionsAsync()
    {
        try
        {
            await _storageService.SaveSessionsAsync(_sessions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving sessions: {ex.Message}");
        }
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            var loadedSettings = await _settingsStorageService.LoadSettingsAsync();
            if (loadedSettings != null)
            {
                _settings.IsCloudMode = loadedSettings.IsCloudMode;
                _settings.IsTestMode = loadedSettings.IsTestMode;
                _settings.SelectedAgentType = loadedSettings.SelectedAgentType;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }
    }

    public async Task SaveSettingsAsync()
    {
        try
        {
            await _settingsStorageService.SaveSettingsAsync(_settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    public async Task<string> ExportChatAsync(string sessionId, string exportPath)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session == null)
        {
            throw new ArgumentException("Session not found", nameof(sessionId));
        }

        await _storageService.ExportChatAsync(session, exportPath);
        return exportPath;
    }

    public async Task SwitchModeAsync(bool isCloudMode)
    {
        _settings.IsCloudMode = isCloudMode;
        
        // Save settings
        await SaveSettingsAsync();
        
        // Reinitialize agent service if not in test mode
        if (!_settings.IsTestMode)
        {
            await InitializeAgentAsync();
        }
        
        NotifyStateChanged();
    }

    public async Task ToggleTestModeAsync(bool isTestMode)
    {
        _settings.IsTestMode = isTestMode;
        
        // Save settings
        await SaveSettingsAsync();
        
        if (!isTestMode)
        {
            await InitializeAgentAsync();
        }
        
        NotifyStateChanged();
    }

    private async Task InitializeAgentAsync()
    {
        try
        {
            // Create agent with selected type
            var configuration = new AgentConfiguration
            {
                AgentType = _settings.SelectedAgentType,
                IsCloudMode = _settings.IsCloudMode
            };
            
            _agentService = new AgentService(_chatAgentFactory, configuration);
            await Task.Run(() => _agentService.Initialize());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing agent: {ex.Message}");
            // Fall back to test mode if initialization fails
            _settings.IsTestMode = true;
        }
    }
    
    public async Task SwitchAgentAsync(AgentType agentType)
    {
        _settings.SelectedAgentType = agentType;
        
        // Save settings
        await SaveSettingsAsync();
        
        // Reinitialize agent service if not in test mode
        if (!_settings.IsTestMode)
        {
            await InitializeAgentAsync();
        }
        
        NotifyStateChanged();
    }

    public List<ChatSession> GetSessions() => _sessions.OrderByDescending(s => s.LastMessageAt).ToList();

    public ChatSession? GetCurrentSession() => _currentSession;

    public ChatSession CreateNewSession(string? agentId = null)
    {
        Agent agent;
        
        if (string.IsNullOrEmpty(agentId))
        {
            // Use the currently selected agent type from settings as default
            agent = _agents.FirstOrDefault(a => a.AgentType == _settings.SelectedAgentType) 
                    ?? _agents.FirstOrDefault(a => a.IsDefault) 
                    ?? _agents.First();
        }
        else
        {
            agent = _agents.FirstOrDefault(a => a.Id == agentId) ?? _agents.First();
            // Update the selected agent type to match the agent for the new session
            _settings.SelectedAgentType = agent.AgentType;
        }
        
        var session = new ChatSession
        {
            Title = "New Chat",
            CreatedAt = DateTime.Now,
            LastMessageAt = DateTime.Now,
            AgentId = agent.Id,
            AgentName = agent.Name
        };
        _sessions.Add(session);
        _currentSession = session;
        
        // Save immediately after creating
        _ = SaveSessionsAsync();
        
        NotifyStateChanged();
        return session;
    }

    public void DeleteSession(string sessionId)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session != null)
        {
            _sessions.Remove(session);
            
            // If we deleted the current session, clear it
            if (_currentSession?.Id == sessionId)
            {
                _currentSession = null;
            }
            
            // Save after deleting
            _ = SaveSessionsAsync();
            
            NotifyStateChanged();
        }
    }

    public void SetCurrentSession(string sessionId)
    {
        _currentSession = _sessions.FirstOrDefault(s => s.Id == sessionId);
        
        // Update the selected agent type to match the session's agent
        if (_currentSession != null)
        {
            var agent = _agents.FirstOrDefault(a => a.Id == _currentSession.AgentId);
            if (agent != null)
            {
                _settings.SelectedAgentType = agent.AgentType;
            }
        }
        
        NotifyStateChanged();
    }

    public async Task<ChatMessage> SendMessageAsync(string content, List<string>? attachedFiles = null, CancellationToken externalCancellationToken = default)
    {
        if (_currentSession == null)
        {
            _currentSession = CreateNewSession();
        }

        var userMessage = new ChatMessage
        {
            Content = content,
            IsUser = true,
            Timestamp = DateTime.Now,
            AttachedFiles = attachedFiles
        };

        _currentSession.Messages.Add(userMessage);
        _currentSession.LastMessageAt = DateTime.Now;

        // Update session title based on first message
        if (_currentSession.Messages.Count == 1)
        {
            _currentSession.Title = content.Length > 50 ? content.Substring(0, 47) + "..." : content;
        }

        NotifyStateChanged();

        // Save sessions after adding user message
        _ = SaveSessionsAsync();

        // Get AI response
        if (_settings.IsTestMode)
        {
            await Task.Delay(500); // Simulate processing time
            
            var aiResponse = new ChatMessage
            {
                IsUser = false,
                Timestamp = DateTime.Now
            };
            
            // Check if user is asking for an adaptive card or specific keywords
            if (content.ToLower().Contains("adaptive card") || 
                content.ToLower().Contains("card") ||
                content.ToLower().Contains("weather") ||
                content.ToLower().Contains("status") ||
                content.ToLower().Contains("profile"))
            {
                aiResponse.AdaptiveCardJson = MockDataProvider.GenerateMockAdaptiveCard(content);
                aiResponse.Content = "Here's the information you requested:";
            }
            else
            {
                aiResponse.Content = MockDataProvider.GenerateMockResponse(content);
            }

            _currentSession.Messages.Add(aiResponse);
            _currentSession.LastMessageAt = DateTime.Now;
            NotifyStateChanged();
            
            // Save sessions after AI response
            _ = SaveSessionsAsync();

            return aiResponse;
        }
        else if (_agentService != null && _agentService.IsInitialized)
        {
            // Use real AI agent with streaming and debounce
            try
            {
                // Mark typing state
                IsAgentTyping = true;
                NotifyStateChanged();

                // Cancel any previous streaming response
                _currentResponseCts?.Cancel();
                _currentResponseCts?.Dispose();

                // Link external cancellation token with internal CTS so either can cancel
                _currentResponseCts = externalCancellationToken == CancellationToken.None
                    ? new CancellationTokenSource()
                    : CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken);

                var token = _currentResponseCts.Token;

                // Create placeholder AI response so UI can show it immediately
                var aiResponse = new ChatMessage
                {
                    Content = string.Empty,
                    IsUser = false,
                    Timestamp = DateTime.Now
                };

                _currentSession.Messages.Add(aiResponse);
                _currentSession.LastMessageAt = DateTime.Now;
                NotifyStateChanged();

                // Debounce settings
                const int debounceMs = 80; // flush UI every ~80ms
                const int flushLength = 32; // or when buffered length reaches this

                var responseSb = new StringBuilder();
                var bufferSb = new StringBuilder();
                var lastFlush = Environment.TickCount;

                await foreach (var chunk in _agentService.StreamMessageAsync(content, token).WithCancellation(token))
                {
                    // Append chunk to buffers
                    responseSb.Append(chunk);
                    bufferSb.Append(chunk);

                    var now = Environment.TickCount;
                    if ((now - lastFlush) >= debounceMs || bufferSb.Length >= flushLength)
                    {
                        // Flush buffered content to the message shown in UI
                        aiResponse.Content = responseSb.ToString();
                        _currentSession.LastMessageAt = DateTime.Now;
                        NotifyStateChanged();

                        bufferSb.Clear();
                        lastFlush = now;
                    }

                    // Respect cancellation
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }

                // Final flush
                aiResponse.Content = responseSb.ToString();
                _currentSession.LastMessageAt = DateTime.Now;
                NotifyStateChanged();

                // Save sessions after AI response
                _ = SaveSessionsAsync();

                return aiResponse;
            }
            catch (OperationCanceledException)
            {
                var errorMessage = new ChatMessage
                {
                    Content = "Response streaming canceled.",
                    IsUser = false,
                    Timestamp = DateTime.Now
                };
                _currentSession.Messages.Add(errorMessage);
                NotifyStateChanged();
                return errorMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting AI response: {ex.Message}");
                var errorMessage = new ChatMessage
                {
                    Content = $"Error: {ex.Message}",
                    IsUser = false,
                    Timestamp = DateTime.Now
                };
                _currentSession.Messages.Add(errorMessage);
                NotifyStateChanged();
                return errorMessage;
            }
            finally
            {
                IsAgentTyping = false;
                NotifyStateChanged();
                _currentResponseCts?.Dispose();
                _currentResponseCts = null;
            }
        }
        else
        {
            // Agent not initialized
            var errorMessage = new ChatMessage
            {
                Content = "AI agent is not initialized. Please check your configuration or enable test mode.",
                IsUser = false,
                Timestamp = DateTime.Now
            };
            _currentSession.Messages.Add(errorMessage);
            NotifyStateChanged();
            return errorMessage;
        }
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}
