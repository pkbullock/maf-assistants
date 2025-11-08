using MAF.Assistants.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MAF.Assistants.Utilities;

/// <summary>
/// Helper methods for working with AgentType enum
/// </summary>
public static class AgentTypeHelper
{
    /// <summary>
    /// Gets the description attribute value for an enum value
    /// </summary>
    public static string GetDescription(this AgentType agentType)
    {
 var field = agentType.GetType().GetField(agentType.ToString());
        if (field == null) return agentType.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? agentType.ToString();
    }

    /// <summary>
    /// Gets the display name for an agent type (first part of description before hyphen)
    /// </summary>
    public static string GetDisplayName(this AgentType agentType)
    {
  var description = GetDescription(agentType);
        var parts = description.Split('-', 2);
      return parts[0].Trim();
    }

    /// <summary>
    /// Gets the full description for an agent type (part after hyphen)
    /// </summary>
    public static string GetFullDescription(this AgentType agentType)
    {
        var description = GetDescription(agentType);
    var parts = description.Split('-', 2);
 return parts.Length > 1 ? parts[1].Trim() : description;
    }

    /// <summary>
    /// Gets all available agent types
    /// </summary>
    public static AgentType[] GetAllAgentTypes()
    {
 return Enum.GetValues<AgentType>();
    }

    /// <summary>
  /// Gets an emoji icon for the agent type
    /// </summary>
    public static string GetIcon(this AgentType agentType)
    {
 return agentType switch
        {
            AgentType.SimpleChat => "??",
            AgentType.CodeExpert => "??",
   AgentType.DataAnalyst => "??",
      AgentType.ContentWriter => "??",
   AgentType.GraphAgent => "??",
            _ => "??"
        };
    }
}
