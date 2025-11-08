using MAF.Assistants.Agents;
using Microsoft.Agents.AI;
using System.Text;

namespace MAF.UI.Services;

public class AgentService
{
    private readonly bool _isCloudMode;
    private readonly SimpleChatAgent _simpleChatAgent;
    private AIAgent? _aiAgent;
    private bool _isInitialized = false;

    public AgentService(SimpleChatAgent simpleChatAgent, bool isCloudMode = true)
    {
        _simpleChatAgent = simpleChatAgent;
        _isCloudMode = isCloudMode;
    }

    public void Initialize()
    {
        try
        {
            // Initialize the AI agent using SimpleChatAgent with the appropriate mode
            _aiAgent = _simpleChatAgent.CreateChatAgent(_isCloudMode);
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

            await foreach (var update in _aiAgent.RunStreamingAsync(message).WithCancellation(cancellationToken))
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
