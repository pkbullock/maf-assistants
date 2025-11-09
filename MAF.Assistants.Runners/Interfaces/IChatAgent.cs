using MAF.Assistants.Models;
using Microsoft.Agents.AI;

namespace MAF.Assistants.Interfaces
{
    /// <summary>
    /// Interface for chat agent implementations
    /// </summary>
    public interface IChatAgent
    {
        /// <summary>
        /// Gets the default configuration for this agent type
        /// </summary>
        static abstract BaseAgentConfiguration DefaultConfiguration { get; }

        /// <summary>
        /// Creates a chat agent instance with the specified cloud mode
        /// </summary>
        /// <param name="isCloudMode">Whether to use cloud or local deployment</param>
        /// <returns>Configured AI agent</returns>
        AIAgent CreateChatAgent(bool isCloudMode = true);

        /// <summary>
        /// Creates a chat agent instance with the specified configuration
        /// </summary>
        /// <param name="configuration">Agent configuration</param>
        /// <returns>Configured AI agent</returns>
        AIAgent CreateChatAgent(AgentConfiguration configuration);
    }
}