using MAF.UI.Models;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using MAF.Assistants.Utilities;
using Microsoft.Agents.AI;

namespace MAF.UI.Services;

public class ChatService
{
    private readonly List<ChatSession> _sessions = new();
    private readonly List<Agent> _agents = new();
    private readonly AppSettings _settings = new();
    private ChatSession? _currentSession;
    private readonly ChatStorageService _storageService;
    private readonly MarkdownService _markdownService;
    private readonly AdaptiveCardService _adaptiveCardService;
    private readonly IChatAgentFactory _chatAgentFactory;
    private AgentService? _agentService;

    public event Action? OnChange;

    public ChatService(ChatStorageService storageService, MarkdownService markdownService, AdaptiveCardService adaptiveCardService, IChatAgentFactory chatAgentFactory)
    {
        _storageService = storageService;
        _markdownService = markdownService;
        _adaptiveCardService = adaptiveCardService;
        _chatAgentFactory = chatAgentFactory;
        
        // Initialize available agents
        InitializeAgents();
  
        // Load saved sessions
        _ = LoadSessionsAsync();
    }

    public AppSettings Settings => _settings;
    
    public MarkdownService Markdown => _markdownService;
    
    public AdaptiveCardService AdaptiveCard => _adaptiveCardService;

    private void InitializeAgents()
    {
        // Initialize agents based on available AgentTypes
        foreach (var agentType in AgentTypeHelper.GetAllAgentTypes())
        {
            _agents.Add(new Agent
            {
                Id = agentType.ToString().ToLower(),
                Name = agentType.GetDisplayName(),
                Description = agentType.GetFullDescription(),
                IconEmoji = agentType.GetIcon(),
                IsDefault = agentType == AgentType.SimpleChat,
                AgentType = agentType
            });
        }
    }

    public List<Agent> GetAvailableAgents() => _agents;

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
                CreateSampleSessions();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading sessions: {ex.Message}");
            CreateSampleSessions();
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
        var agent = string.IsNullOrEmpty(agentId) 
            ? _agents.FirstOrDefault(a => a.IsDefault) ?? _agents.First()
            : _agents.FirstOrDefault(a => a.Id == agentId) ?? _agents.First();
        
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
        NotifyStateChanged();
    }

    public async Task<ChatMessage> SendMessageAsync(string content, List<string>? attachedFiles = null)
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
            // Use real AI agent
            try
            {
                var response = await _agentService.SendMessageAsync(content);
                var aiResponse = new ChatMessage
                {
                    Content = response,
                    IsUser = false,
                    Timestamp = DateTime.Now
                };

                _currentSession.Messages.Add(aiResponse);
                _currentSession.LastMessageAt = DateTime.Now;
                NotifyStateChanged();
                
                // Save sessions after AI response
                _ = SaveSessionsAsync();

                return aiResponse;
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

    private void CreateSampleSessions()
    {
        // Add some sample chat history
        var session1 = new ChatSession
        {
            Title = "test",
            CreatedAt = DateTime.Now.AddDays(-5).AddHours(-10).AddMinutes(-19),
            LastMessageAt = DateTime.Now.AddDays(-5).AddHours(-10).AddMinutes(-19)
        };
        session1.Messages.Add(new ChatMessage { Content = "test", IsUser = true });
        session1.Messages.Add(new ChatMessage { Content = "This is a test response.", IsUser = false });

        var session2 = new ChatSession
        {
            Title = "test",
            CreatedAt = DateTime.Now.AddDays(-5).AddHours(-9).AddMinutes(-58),
            LastMessageAt = DateTime.Now.AddDays(-5).AddHours(-9).AddMinutes(-58)
        };
        session2.Messages.Add(new ChatMessage { Content = "test", IsUser = true });

        var session3 = new ChatSession
        {
            Title = "Explain quantum computing in simple terms",
            CreatedAt = DateTime.Now.AddMonths(-11).AddDays(-8).AddHours(-15).AddMinutes(-37),
            LastMessageAt = DateTime.Now.AddMonths(-11).AddDays(-8).AddHours(-15).AddMinutes(-37)
        };

        var session4 = new ChatSession
        {
            Title = "Help me write a professional email",
            CreatedAt = DateTime.Now.AddMonths(-11).AddDays(-8).AddHours(-14).AddMinutes(-40),
            LastMessageAt = DateTime.Now.AddMonths(-11).AddDays(-8).AddHours(-14).AddMinutes(-40)
        };

        var session5 = new ChatSession
        {
            Title = "API design best practices",
            CreatedAt = DateTime.Now.AddMonths(-11).AddDays(-8).AddHours(-11).AddMinutes(-53),
            LastMessageAt = DateTime.Now.AddMonths(-11).AddDays(-8).AddHours(-11).AddMinutes(-53)
        };

        _sessions.AddRange(new[] { session1, session2, session3, session4, session5 });
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
