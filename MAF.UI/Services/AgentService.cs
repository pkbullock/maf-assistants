using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using Microsoft.Agents.AI;
using System.Text;
using System.Runtime.CompilerServices;

namespace MAF.UI.Services;

public class AgentService
{
    private readonly IChatAgentFactory _chatAgentFactory;
    private readonly AgentConfiguration _configuration;
    private AIAgent? _aiAgent;
    private AgentSession? _session;
    private bool _isInitialized = false;

    public AgentService(IChatAgentFactory chatAgentFactory, bool isCloudMode = true)
        : this(chatAgentFactory, new AgentConfiguration { IsCloudMode = isCloudMode })
    {
    }

    public AgentService(IChatAgentFactory chatAgentFactory, AgentConfiguration configuration)
    {
        _chatAgentFactory = chatAgentFactory;
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Initialize()
    {
        try
        {
            // Initialize the AI agent using the factory with the configuration
            _aiAgent = _chatAgentFactory.CreateChatAgent(_configuration);
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing agent: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Streams partial text updates from the AI agent as they are produced.
    /// </summary>
    public async IAsyncEnumerable<string> StreamMessageAsync(string message, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!_isInitialized || _aiAgent == null)
        {
            throw new InvalidOperationException("Agent not initialized. Call Initialize() first.");
        }

        // Create session lazily on first use to avoid sync-over-async anti-pattern
        _session ??= await _aiAgent.CreateSessionAsync(cancellationToken);

        // Opportunity to customize options based on configuration
        ChatClientAgentRunOptions agentRunOptions = new(new()
        {
            MaxOutputTokens = _configuration.DefaultMaxTokens
        });

        await foreach (var update in _aiAgent.RunStreamingAsync(message, _session, agentRunOptions).WithCancellation(cancellationToken))
        {
            // The update is an AgentRunResponseUpdate; convert to text for UI streaming
            yield return update?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Compatibility helper that collects the stream into a single string
    /// </summary>
    public async Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();
        await foreach (var chunk in StreamMessageAsync(message, cancellationToken).WithCancellation(cancellationToken))
        {
            sb.Append(chunk);
        }
        return sb.ToString();
    }

    public bool IsInitialized => _isInitialized;
}
