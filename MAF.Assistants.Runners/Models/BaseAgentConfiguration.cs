using System;

namespace MAF.Assistants.Models
{
    /// <summary>
    /// Base configuration class for all agent types with common properties
    /// </summary>
    public class BaseAgentConfiguration
    {
        /// <summary>
        /// The agent type this configuration is for
        /// </summary>
        public AgentType AgentType { get; set; }

        /// <summary>
        /// System instructions for the agent
        /// </summary>
        public string Instructions { get; set; } = string.Empty;

        /// <summary>
        /// Name of the agent
        /// </summary>
        public string AgentName { get; set; } = "AI Assistant";

        /// <summary>
        /// Description of the agent's capabilities
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Icon emoji for the agent
        /// </summary>
        public string IconEmoji { get; set; } = "🤖";

        /// <summary>
        /// Whether this agent is available for selection
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Default cloud mode setting
        /// </summary>
        public bool DefaultIsCloudMode { get; set; } = true;

        public BaseAgentConfiguration() { }

        public BaseAgentConfiguration(
            AgentType agentType,
            string instructions,
            string agentName,
            string description,
            string iconEmoji)
        {
            AgentType = agentType;
            Instructions = instructions;
            AgentName = agentName;
            Description = description;
            IconEmoji = iconEmoji;
        }
    }
}
