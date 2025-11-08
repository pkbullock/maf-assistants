using MAF.UI.Models;

namespace MAF.UI.Services;

public class ChatService
{
    private readonly List<ChatSession> _sessions = new();
    private readonly AppSettings _settings = new();
    private ChatSession? _currentSession;
    private readonly ChatStorageService _storageService;
    private readonly MarkdownService _markdownService;
    private AgentService? _agentService;

    public event Action? OnChange;

    public ChatService(ChatStorageService storageService, MarkdownService markdownService)
    {
        _storageService = storageService;
        _markdownService = markdownService;
        
        // Load saved sessions
        _ = LoadSessionsAsync();
    }

    public AppSettings Settings => _settings;
    
    public MarkdownService Markdown => _markdownService;

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

    public void SwitchMode(bool isCloudMode)
    {
        _settings.IsCloudMode = isCloudMode;
        
        // Reinitialize agent service if not in mock mode
        if (!_settings.IsMockMode)
        {
            InitializeAgent();
        }
        
        NotifyStateChanged();
    }

    public void ToggleMockMode(bool isMockMode)
    {
        _settings.IsMockMode = isMockMode;
        
        if (!isMockMode)
        {
            InitializeAgent();
        }
        
        NotifyStateChanged();
    }

    private void InitializeAgent()
    {
        try
        {
            _agentService = new AgentService(_settings.IsCloudMode);
            _agentService.Initialize();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing agent: {ex.Message}");
            // Fall back to mock mode if initialization fails
            _settings.IsMockMode = true;
        }
    }

    public List<ChatSession> GetSessions() => _sessions.OrderByDescending(s => s.LastMessageAt).ToList();

    public ChatSession? GetCurrentSession() => _currentSession;

    public ChatSession CreateNewSession()
    {
        var session = new ChatSession
        {
            Title = "New Chat",
            CreatedAt = DateTime.Now,
            LastMessageAt = DateTime.Now
        };
        _sessions.Add(session);
        _currentSession = session;
        NotifyStateChanged();
        return session;
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
        if (_settings.IsMockMode)
        {
            await Task.Delay(500); // Simulate processing time
            
            var aiResponse = new ChatMessage
            {
                Content = GenerateMockResponse(content),
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
                Content = "AI agent is not initialized. Please check your configuration or enable mock mode.",
                IsUser = false,
                Timestamp = DateTime.Now
            };
            _currentSession.Messages.Add(errorMessage);
            NotifyStateChanged();
            return errorMessage;
        }
    }

    private string GenerateMockResponse(string userMessage)
    {
        var responses = new[]
        {
            $"I understand you're asking about: '{userMessage}'. In mock mode, I can provide simulated responses.",
            "That's an interesting question! When connected to the backend, I'll be able to provide more detailed answers.",
            $"Based on your message about '{userMessage}', here's a mock response to help you develop the UI.",
            "This is a simulated response from the AI assistant in mock mode."
        };

        return responses[new Random().Next(responses.Length)];
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
