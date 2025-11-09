using MAF.Assistants.Factories;
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
    /// Gets the display name for an agent type from the agent's default configuration
    /// </summary>
    public static string GetDisplayName(this AgentType agentType)
    {
      try
        {
  var config = AgentRegistry.GetDefaultConfiguration(agentType);
      return config.AgentName;
        }
        catch
        {
       // Fallback to enum name if configuration not available
    return agentType.ToString();
  }
    }

    /// <summary>
    /// Gets the full description for an agent type from the agent's default configuration
    /// </summary>
    public static string GetFullDescription(this AgentType agentType)
    {
try
    {
  var config = AgentRegistry.GetDefaultConfiguration(agentType);
   return config.Description;
        }
        catch
     {
     // Fallback to description attribute
 var description = GetDescription(agentType);
   var parts = description.Split('-', 2);
     return parts.Length > 1 ? parts[1].Trim() : description;
}
    }

    /// <summary>
    /// Gets all available agent types (only registered ones)
    /// </summary>
    public static AgentType[] GetAllAgentTypes()
    {
  return AgentRegistry.GetRegisteredTypes().ToArray();
    }

  /// <summary>
    /// Gets an emoji icon for the agent type from the agent's default configuration
    /// </summary>
 public static string GetIcon(this AgentType agentType)
    {
        try
     {
      var config = AgentRegistry.GetDefaultConfiguration(agentType);
      return config.IconEmoji;
        }
        catch
   {
   // Fallback icons if configuration not available
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
    
    /// <summary>
    /// Checks if an agent type is available
    /// </summary>
    public static bool IsAvailable(this AgentType agentType)
{
    return AgentRegistry.IsRegistered(agentType);
    }
}
