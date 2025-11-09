# Agent Class Architecture

## Overview

The MAF.Assistants framework now uses a **class-based agent architecture** where each `AgentType` enum value maps to a specific agent class that contains its own pre-defined configuration and creation logic.

## Architecture Pattern

### Core Components

```
MAF.Assistants/
??? Models/
?   ??? AgentType.cs# Enum defining all agent types
?   ??? BaseAgentConfiguration.cs     # Base configuration for all agents
?   ??? AgentConfiguration.cs         # Runtime configuration
??? Interfaces/
?   ??? IChatAgent.cs    # Interface for agent implementations
?   ??? IChatAgentFactory.cs          # Factory interface
??? Agents/
?   ??? SimpleChatAgent.cs            # General purpose agent
?   ??? HumourChatAgent.cs            # Joke-telling agent
?   ??? CodeExpertAgent.cs            # Programming expert agent
?   ??? DataAnalystAgent.cs           # Data analysis agent
?   ??? ContentWriterAgent.cs      # Writing specialist agent
?   ??? GraphAgent.cs      # Microsoft 365 integrated agent
??? Factories/
    ??? AgentRegistry.cs          # Maps AgentType ? Agent Class
    ??? ChatAgentFactory.cs           # Main factory implementation
```

## How It Works

### 1. Agent Type Enum

Each agent type is defined in the `AgentType` enum:

```csharp
public enum AgentType
{
    SimpleChat = 0,
    HumourWriter = 1,
    CodeExpert = 2,
    DataAnalyst = 3,
    ContentWriter = 4,
    GraphAgent = 5
}
```

### 2. Agent Classes

Each agent type has a corresponding class that implements `IChatAgent`:

```csharp
public class SimpleChatAgent : IChatAgent
{
  // Static default configuration
    public static BaseAgentConfiguration DefaultConfiguration => new()
    {
        AgentType = AgentType.SimpleChat,
        Instructions = "You are a helpful and friendly AI assistant...",
      AgentName = "General Assistant",
        Description = "General purpose conversational AI",
      IconEmoji = "??",
        DefaultIsCloudMode = true
    };

    // Creates agent with simple cloud mode parameter
    public AIAgent CreateChatAgent(bool isCloudMode = true)
 {
      var config = new AgentConfiguration
{
     AgentType = DefaultConfiguration.AgentType,
 CustomInstructions = DefaultConfiguration.Instructions,
            AgentName = DefaultConfiguration.AgentName,
      IsCloudMode = isCloudMode
        };
        return CreateChatAgent(config);
    }

    // Creates agent with full configuration
    public AIAgent CreateChatAgent(AgentConfiguration configuration)
    {
        // Uses configuration or falls back to defaults
    string instructions = string.IsNullOrEmpty(configuration.CustomInstructions)
? DefaultConfiguration.Instructions
          : configuration.CustomInstructions;

        string agentName = string.IsNullOrEmpty(configuration.AgentName)
        ? DefaultConfiguration.AgentName
            : configuration.AgentName;

        // Create the ChatClient and agent
        ChatClient client = configuration.IsCloudMode 
            ? Clients.GetAzureChat() 
  : Clients.GetLocalFoundryChat();

        return client.CreateAIAgent(instructions: instructions, name: agentName);
    }
}
```

### 3. Agent Registry

The `AgentRegistry` maps each `AgentType` to its agent class:

```csharp
public static class AgentRegistry
{
    private static readonly Dictionary<AgentType, Func<bool, AIAgent>> _simpleAgentFactories = new()
    {
     { AgentType.SimpleChat, (isCloudMode) => new SimpleChatAgent().CreateChatAgent(isCloudMode) },
        { AgentType.HumourWriter, (isCloudMode) => new HumourChatAgent().CreateChatAgent(isCloudMode) },
      { AgentType.CodeExpert, (isCloudMode) => new CodeExpertAgent().CreateChatAgent(isCloudMode) },
        // ... more agents
    };

    // Creates agent by type and cloud mode
    public static AIAgent CreateAgent(AgentType agentType, bool isCloudMode = true)
    {
        if (_simpleAgentFactories.TryGetValue(agentType, out var factory))
     {
            return factory(isCloudMode);
        }
        throw new ArgumentException($"No agent registered for type: {agentType}");
    }

    // Creates agent by configuration
    public static AIAgent CreateAgent(AgentConfiguration configuration)
    {
        // Similar pattern with _configAgentFactories
    }

    // Gets default configuration for an agent type
    public static BaseAgentConfiguration GetDefaultConfiguration(AgentType agentType)
    {
return agentType switch
        {
    AgentType.SimpleChat => SimpleChatAgent.DefaultConfiguration,
  AgentType.HumourWriter => HumourChatAgent.DefaultConfiguration,
          // ... more agents
 };
    }
}
```

### 4. Factory Usage

The `ChatAgentFactory` delegates to the registry:

```csharp
public class ChatAgentFactory : IChatAgentFactory
{
 public AIAgent CreateChatAgent(AgentType agentType, bool isCloudMode = true)
    {
 return AgentRegistry.CreateAgent(agentType, isCloudMode);
    }

    public AIAgent CreateChatAgent(AgentConfiguration configuration)
    {
    return AgentRegistry.CreateAgent(configuration);
    }
}
```

## Usage Examples

### Basic Usage

```csharp
// Inject the factory
var factory = serviceProvider.GetService<IChatAgentFactory>();

// Create a simple chat agent (cloud mode)
var agent = factory.CreateChatAgent(AgentType.SimpleChat);

// Create a code expert agent (local mode)
var codeExpert = factory.CreateChatAgent(AgentType.CodeExpert, isCloudMode: false);

// Use the agent
var response = await agent.RunAsync("Hello!");
```

### Custom Configuration

```csharp
// Create with custom instructions
var config = new AgentConfiguration
{
    AgentType = AgentType.ContentWriter,
    CustomInstructions = "You are a technical writer specializing in API documentation.",
    AgentName = "API Doc Writer",
    IsCloudMode = true
};

var customAgent = factory.CreateChatAgent(config);
```

### Accessing Agent Information

```csharp
// Get default configuration for an agent type
var defaultConfig = AgentRegistry.GetDefaultConfiguration(AgentType.DataAnalyst);
Console.WriteLine($"Name: {defaultConfig.AgentName}");
Console.WriteLine($"Icon: {defaultConfig.IconEmoji}");
Console.WriteLine($"Description: {defaultConfig.Description}");

// Check if agent type is registered
bool isAvailable = AgentRegistry.IsRegistered(AgentType.GraphAgent);

// Get all registered types
var allTypes = AgentRegistry.GetRegisteredTypes();
```

## Available Agents

| AgentType | Class | Default Name | Icon | Purpose |
|-----------|-------|--------------|------|---------|
| `SimpleChat` | `SimpleChatAgent` | General Assistant | ?? | General purpose conversations |
| `HumourWriter` | `HumourChatAgent` | JokeAI | ?? | Jokes and humorous responses |
| `CodeExpert` | `CodeExpertAgent` | Code Expert | ?? | Programming and development |
| `DataAnalyst` | `DataAnalystAgent` | Data Analyst | ?? | Data analysis and visualization |
| `ContentWriter` | `ContentWriterAgent` | Content Writer | ?? | Creative and technical writing |
| `GraphAgent` | `GraphAgent` | Graph Agent | ?? | Microsoft 365 integration |

## Adding a New Agent Type

### Step 1: Add to AgentType Enum

```csharp
// In Models/AgentType.cs
public enum AgentType
{
    // ... existing types
    
    [Description("New Agent - Description here")]
    NewAgent = 6
}
```

### Step 2: Create Agent Class

```csharp
// In Agents/NewAgent.cs
public class NewAgent : IChatAgent
{
    public static BaseAgentConfiguration DefaultConfiguration => new()
    {
        AgentType = AgentType.NewAgent,
        Instructions = "Your agent instructions...",
        AgentName = "Agent Name",
   Description = "Agent description",
        IconEmoji = "??",
        DefaultIsCloudMode = true
    };

    public AIAgent CreateChatAgent(bool isCloudMode = true)
    {
    var config = new AgentConfiguration
        {
         AgentType = DefaultConfiguration.AgentType,
   CustomInstructions = DefaultConfiguration.Instructions,
  AgentName = DefaultConfiguration.AgentName,
   IsCloudMode = isCloudMode
        };
        return CreateChatAgent(config);
    }

    public AIAgent CreateChatAgent(AgentConfiguration configuration)
    {
     // Implementation as shown in pattern above
    }
}
```

### Step 3: Register in AgentRegistry

```csharp
// In Factories/AgentRegistry.cs

// Add to _simpleAgentFactories
{ AgentType.NewAgent, (isCloudMode) => new NewAgent().CreateChatAgent(isCloudMode) }

// Add to _configAgentFactories
{ AgentType.NewAgent, (config) => new NewAgent().CreateChatAgent(config) }

// Add to GetDefaultConfiguration switch
AgentType.NewAgent => NewAgent.DefaultConfiguration,
```

### Step 4: Test

```csharp
var agent = factory.CreateChatAgent(AgentType.NewAgent);
var response = await agent.RunAsync("Test message");
```

## Benefits of This Architecture

1. **Encapsulation**: Each agent class contains its own configuration and creation logic
2. **Type Safety**: Enum-based selection prevents invalid agent types
3. **Extensibility**: Easy to add new agents without modifying existing code
4. **Testability**: Each agent can be tested independently
5. **Maintainability**: Agent-specific logic is isolated in its own class
6. **Flexibility**: Supports both simple creation and custom configuration
7. **Discoverability**: Registry provides runtime introspection

## Configuration Hierarchy

The system supports a configuration hierarchy:

1. **Default Configuration** (in agent class)
   - Defined as static property in each agent class
   - Contains type-specific defaults

2. **Runtime Configuration** (AgentConfiguration)
   - Can override default instructions and name
   - Specifies cloud/local mode

3. **Fallback Logic**
   - If CustomInstructions is empty ? use DefaultConfiguration.Instructions
   - If AgentName is empty ? use DefaultConfiguration.AgentName

Example:

```csharp
// Uses all defaults
var agent1 = factory.CreateChatAgent(AgentType.CodeExpert);

// Overrides instructions only
var config = new AgentConfiguration
{
    AgentType = AgentType.CodeExpert,
    CustomInstructions = "Focus on Python programming",
    // AgentName will use default "Code Expert"
};
var agent2 = factory.CreateChatAgent(config);

// Overrides everything
var fullConfig = new AgentConfiguration
{
    AgentType = AgentType.CodeExpert,
    CustomInstructions = "Focus on Rust programming",
    AgentName = "Rust Expert",
    IsCloudMode = false
};
var agent3 = factory.CreateChatAgent(fullConfig);
```

## Best Practices

1. **Keep configurations in agent classes** - Don't duplicate configuration in multiple places
2. **Use meaningful instructions** - Make agent behavior clear in the instructions
3. **Test with both cloud and local** - Ensure agents work in both modes
4. **Document agent capabilities** - Include clear descriptions
5. **Follow naming conventions** - Agent classes should end with "Agent"
6. **Register immediately** - Add new agents to the registry as soon as they're created
7. **Use appropriate emojis** - Choose icons that represent the agent's purpose

## Troubleshooting

### Agent Not Found
**Error**: `ArgumentException: No agent registered for type: AgentType.NewAgent`

**Solution**: Ensure the agent is registered in `AgentRegistry._simpleAgentFactories` and `_configAgentFactories`

### Static Abstract Member Issues
**Error**: `CS8920: The interface 'IChatAgent' cannot be used as type argument`

**Solution**: This was resolved by using `Func<>` delegates in the registry instead of storing `IChatAgent` instances

### Configuration Not Applied
**Issue**: Custom configuration not being used

**Solution**: Check the `CreateChatAgent(AgentConfiguration)` implementation in your agent class - ensure it uses the configuration parameter correctly

### Cloud/Local Mode Not Working
**Issue**: Agent always uses one mode

**Solution**: Verify `Clients.GetAzureChat()` and `Clients.GetLocalFoundryChat()` are properly configured in user secrets

## Implementation Complete! ?

The agent class architecture is now fully implemented with:
- ? 6 agent types with dedicated classes
- ? Registry system for type mapping
- ? Configuration hierarchy support
- ? Factory delegation pattern
- ? Full UI integration
- ? Comprehensive error handling
- ? Extensible design for future agents
