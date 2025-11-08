using MAF.UI.Models;
using Microsoft.Agents.AI;
using MAF.Assistants.Agents;

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
    private readonly SimpleChatAgent _simpleChatAgent;
    private AgentService? _agentService;

    public event Action? OnChange;

    public ChatService(ChatStorageService storageService, MarkdownService markdownService, AdaptiveCardService adaptiveCardService, SimpleChatAgent simpleChatAgent)
    {
        _storageService = storageService;
      _markdownService = markdownService;
        _adaptiveCardService = adaptiveCardService;
        _simpleChatAgent = simpleChatAgent;
        
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
        _agents.Add(new Agent
        {
            Id = "default",
            Name = "General Assistant",
            Description = "A helpful AI assistant for general queries",
            IconEmoji = "🤖",
            IsDefault = true
        });
        
        _agents.Add(new Agent
        {
            Id = "code-expert",
            Name = "Code Expert",
            Description = "Specialized in programming and software development",
            IconEmoji = "💻"
        });
        
        _agents.Add(new Agent
        {
            Id = "data-analyst",
            Name = "Data Analyst",
            Description = "Expert in data analysis and visualization",
            IconEmoji = "📊"
        });
        
        _agents.Add(new Agent
        {
            Id = "writer",
            Name = "Content Writer",
            Description = "Specialized in creative and technical writing",
            IconEmoji = "✍️"
        });
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
        
        // Reinitialize agent service if not in mock mode
        if (!_settings.IsMockMode)
        {
            await InitializeAgentAsync();
        }
        
        NotifyStateChanged();
    }

    public async Task ToggleMockModeAsync(bool isMockMode)
    {
        _settings.IsMockMode = isMockMode;
        
        if (!isMockMode)
        {
            await InitializeAgentAsync();
        }
        
        NotifyStateChanged();
    }

    private async Task InitializeAgentAsync()
    {
        try
        {
            _agentService = new AgentService(_simpleChatAgent, _settings.IsCloudMode);
            await Task.Run(() => _agentService.Initialize());
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
        if (_settings.IsMockMode)
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
                aiResponse.AdaptiveCardJson = GenerateMockAdaptiveCard(content);
                aiResponse.Content = "Here's the information you requested:";
            }
            else
            {
                aiResponse.Content = GenerateMockResponse(content);
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

    private string GenerateMockAdaptiveCard(string userMessage)
    {
        // Generate different types of adaptive cards based on the user's message
        if (userMessage.ToLower().Contains("weather"))
        {
            return @"{
                ""type"": ""AdaptiveCard"",
                ""version"": ""1.5"",
                ""body"": [
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Seattle Weather"",
                        ""size"": ""Large"",
                        ""weight"": ""Bolder""
                    },
                    {
                        ""type"": ""ColumnSet"",
                        ""columns"": [
                            {
                                ""type"": ""Column"",
                                ""width"": ""auto"",
                                ""items"": [
                                    {
                                        ""type"": ""Image"",
                                        ""url"": ""https://adaptivecards.io/content/weather-sunny.png"",
                                        ""size"": ""Small""
                                    }
                                ]
                            },
                            {
                                ""type"": ""Column"",
                                ""width"": ""stretch"",
                                ""items"": [
                                    {
                                        ""type"": ""TextBlock"",
                                        ""text"": ""72°F"",
                                        ""size"": ""ExtraLarge""
                                    },
                                    {
                                        ""type"": ""TextBlock"",
                                        ""text"": ""Partly Cloudy"",
                                        ""spacing"": ""None""
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        ""type"": ""FactSet"",
                        ""facts"": [
                            {
                                ""title"": ""Humidity"",
                                ""value"": ""65%""
                            },
                            {
                                ""title"": ""Wind"",
                                ""value"": ""8 mph NW""
                            },
                            {
                                ""title"": ""Visibility"",
                                ""value"": ""10 mi""
                            }
                        ]
                    }
                ]
            }";
        }
        else if (userMessage.ToLower().Contains("status") || userMessage.ToLower().Contains("project"))
        {
            return @"{
                ""type"": ""AdaptiveCard"",
                ""version"": ""1.5"",
                ""body"": [
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Project Status Update"",
                        ""size"": ""Large"",
                        ""weight"": ""Bolder""
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""MAF Assistants Development"",
                        ""color"": ""Accent"",
                        ""spacing"": ""None""
                    },
                    {
                        ""type"": ""FactSet"",
                        ""facts"": [
                            {
                                ""title"": ""Status"",
                                ""value"": ""In Progress""
                            },
                            {
                                ""title"": ""Completion"",
                                ""value"": ""75%""
                            },
                            {
                                ""title"": ""Next Milestone"",
                                ""value"": ""Q1 2025""
                            },
                            {
                                ""title"": ""Team Members"",
                                ""value"": ""5""
                            }
                        ]
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Recent achievements: Adaptive Card support, Mock mode, Chat UI improvements"",
                        ""wrap"": true,
                        ""spacing"": ""Medium""
                    }
                ],
                ""actions"": [
                    {
                        ""type"": ""Action.OpenUrl"",
                        ""title"": ""View Details"",
                        ""url"": ""https://github.com/pkbullock/maf-assistants""
                    }
                ]
            }";
        }
        else if (userMessage.ToLower().Contains("profile"))
        {
            return @"{
                ""type"": ""AdaptiveCard"",
                ""version"": ""1.5"",
                ""body"": [
                    {
                        ""type"": ""ColumnSet"",
                        ""columns"": [
                            {
                                ""type"": ""Column"",
                                ""width"": ""auto"",
                                ""items"": [
                                    {
                                        ""type"": ""Image"",
                                        ""url"": ""https://adaptivecards.io/content/logo-256.png"",
                                        ""size"": ""Small"",
                                        ""style"": ""Person""
                                    }
                                ]
                            },
                            {
                                ""type"": ""Column"",
                                ""width"": ""stretch"",
                                ""items"": [
                                    {
                                        ""type"": ""TextBlock"",
                                        ""text"": ""MAF Assistant"",
                                        ""weight"": ""Bolder"",
                                        ""size"": ""Large""
                                    },
                                    {
                                        ""type"": ""TextBlock"",
                                        ""text"": ""AI-Powered Development Assistant"",
                                        ""spacing"": ""None""
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        ""type"": ""FactSet"",
                        ""facts"": [
                            {
                                ""title"": ""Version"",
                                ""value"": ""1.0.0""
                            },
                            {
                                ""title"": ""Framework"",
                                ""value"": "".NET 9.0""
                            },
                            {
                                ""title"": ""UI"",
                                ""value"": ""Blazor Server""
                            }
                        ]
                    }
                ]
            }";
        }
        else
        {
            // Default card
            return @"{
                ""type"": ""AdaptiveCard"",
                ""version"": ""1.5"",
                ""body"": [
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Adaptive Card Example"",
                        ""size"": ""Large"",
                        ""weight"": ""Bolder""
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""This is a sample adaptive card response in mock mode."",
                        ""wrap"": true
                    },
                    {
                        ""type"": ""FactSet"",
                        ""facts"": [
                            {
                                ""title"": ""Feature"",
                                ""value"": ""Adaptive Cards""
                            },
                            {
                                ""title"": ""Status"",
                                ""value"": ""Active""
                            },
                            {
                                ""title"": ""Mode"",
                                ""value"": ""Mock""
                            }
                        ]
                    }
                ],
                ""actions"": [
                    {
                        ""type"": ""Action.OpenUrl"",
                        ""title"": ""Learn More"",
                        ""url"": ""https://adaptivecards.io""
                    }
                ]
            }";
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
