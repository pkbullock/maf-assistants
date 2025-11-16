using MAF.Assistants.Interfaces;
using MAF.Assistants.Models;
using Microsoft.Agents.AI;
using System.Text;

namespace MAF.UI.Services;

public class AgentService
{
    private readonly IChatAgentFactory _chatAgentFactory;
    private readonly AgentConfiguration _configuration;
    private AIAgent? _aiAgent;
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

    public async Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        if (!_isInitialized || _aiAgent == null)
        {
            throw new InvalidOperationException("Agent not initialized. Call Initialize() first.");
        }

        try
        {
            // Use the AI agent to get a streaming response and collect it
            var responseBuilder = new StringBuilder();

            //TODO: Opportunity to customize options based on configuration
            ChatClientAgentRunOptions agentRunOptions = new(new()
            {
               MaxOutputTokens = _configuration.DefaultMaxTokens
            });

           
            await foreach (var update in _aiAgent.RunStreamingAsync(message, options: agentRunOptions).WithCancellation(cancellationToken))
            {
                responseBuilder.Append(update);
            }

            return responseBuilder.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
            throw;
        }
    }

    public bool IsInitialized => _isInitialized;
}
