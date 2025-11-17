using MAF.UI.Models;

namespace MAF.UI.Services;

/// <summary>
/// Provides mock data for testing and development purposes.
/// Contains sample responses and adaptive cards to help develop the UI without a backend connection.
/// </summary>
public static class MockDataProvider
{
    /// <summary>
    /// Generates a mock response based on the user's message.
    /// </summary>
    /// <param name="userMessage">The user's input message</param>
    /// <returns>A simulated AI response</returns>
    public static string GenerateMockResponse(string userMessage)
    {
        var responses = new[]
        {
            $"I understand you're asking about: '{userMessage}'. In test mode, I can provide simulated responses.",
            "That's an interesting question! When connected to the backend, I'll be able to provide more detailed answers.",
            $"Based on your message about '{userMessage}', here's a mock response to help you develop the UI.",
            "This is a simulated response from the AI assistant in test mode."
        };

        return responses[new Random().Next(responses.Length)];
    }

    /// <summary>
    /// Generates a mock adaptive card based on the user's message.
    /// Returns different card types based on keywords in the message.
    /// </summary>
    /// <param name="userMessage">The user's input message</param>
    /// <returns>JSON string representing an adaptive card</returns>
    public static string GenerateMockAdaptiveCard(string userMessage)
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
                        ""text"": ""Recent achievements: Adaptive Card support, Test mode, Chat UI improvements"",
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
                        ""text"": ""This is a sample adaptive card response in test mode."",
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
                                ""value"": ""Test""
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


    /// <summary>
    /// Creates and returns a set of sample chat sessions for demonstration or testing purposes.
    /// </summary>
    /// <remarks>This method is intended for use in scenarios where example chat data is needed, such as UI
    /// previews, automated tests, or initial application setup. The returned sessions include a mix of populated and
    /// empty message histories to illustrate different states.</remarks>
    /// <returns>An array of <see cref="ChatSession"/> objects containing predefined sample chat sessions. The array will contain
    /// five sessions with various titles and message histories.</returns>
    public static ChatSession[] CreateSampleSessions()
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

        return new[] { session1, session2, session3, session4, session5 };
    }
}
