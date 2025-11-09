# Agent Type System

This document describes the new agent type system that allows users to select from different specialized AI agents.

## Overview

The MAF.Assistants framework now supports multiple agent types, each with specialized capabilities and instructions. Users can select their preferred agent type from the UI settings.

## Available Agent Types

### 1. SimpleChat (`AgentType.SimpleChat`)
- **Icon**: ??
- **Name**: Simple Chat Agent
- **Description**: General purpose conversational AI
- **Use Case**: Everyday conversations, general questions, and assistance
- **Instructions**: "You are a helpful and friendly AI assistant. You provide clear, accurate, and concise responses to user questions."

### 2. CodeExpert (`AgentType.CodeExpert`)
- **Icon**: ??
- **Name**: Code Expert
- **Description**: Specialized in programming and software development
- **Use Case**: Code reviews, debugging, architecture advice, programming questions
- **Instructions**: "You are an expert software developer and architect with deep knowledge of programming languages, design patterns, and best practices. You provide detailed code examples, explain complex concepts clearly, and help debug issues."

### 3. DataAnalyst (`AgentType.DataAnalyst`)
- **Icon**: ??
- **Name**: Data Analyst
- **Description**: Expert in data analysis and visualization
- **Use Case**: Data interpretation, statistical analysis, visualization recommendations
- **Instructions**: "You are a data analyst expert specializing in data analysis, visualization, and statistical interpretation. You help users understand their data, create insights, and recommend appropriate analysis techniques."

### 4. ContentWriter (`AgentType.ContentWriter`)
- **Icon**: ??
- **Name**: Content Writer
- **Description**: Specialized in creative and technical writing
- **Use Case**: Content creation, editing, copywriting, documentation
- **Instructions**: "You are a professional content writer skilled in both creative and technical writing. You help craft engaging content, improve writing quality, and adapt tone and style to different audiences."

### 5. GraphAgent (`AgentType.GraphAgent`)
- **Icon**: ??
- **Name**: Graph Agent
- **Description**: Integrated with Microsoft 365 services
- **Use Case**: Microsoft 365 operations, email, files, calendar, Teams integration
- **Instructions**: "You are an AI assistant with access to Microsoft 365 services through Microsoft Graph. You can help with emails, files, calendar, Teams, and other Microsoft 365 tasks."

## Architecture

### Core Components

```
MAF.Assistants/
??? Models/
?   ??? AgentType.cs              # Enum defining agent types
?   ??? AgentConfiguration.cs# Configuration for agent creation
??? Interfaces/
?   ??? IChatAgentFactory.cs      # Factory interface with agent type support
??? Factories/
?   ??? ChatAgentFactory.cs       # Factory implementation
??? Utilities/
    ??? AgentTypeHelper.cs        # Helper methods for AgentType enum

MAF.UI/
??? Models/
?   ??? Agent.cs           # UI agent model with AgentType
?   ??? AppSettings.cs     # Settings with SelectedAgentType
??? Services/
?   ??? ChatService.cs            # Updated to use agent types
?   ??? AgentService.cs     # Updated to accept configuration
??? Components/Pages/
    ??? Settings.razor            # UI for agent selection
```

## Usage Examples

### Creating an Agent Programmatically

```csharp
// Method 1: Using AgentType enum
var factory = new ChatAgentFactory();
var agent = factory.CreateChatAgent(AgentType.CodeExpert, isCloudMode: true);

// Method 2: Using AgentConfiguration
var config = new AgentConfiguration
{
    AgentType = AgentType.DataAnalyst,
    IsCloudMode = true,
AgentName = "Data Insights AI"
};
var agent = factory.CreateChatAgent(config);

// Method 3: Custom instructions
var customConfig = new AgentConfiguration
{
    AgentType = AgentType.SimpleChat,
    CustomInstructions = "You are a friendly assistant specialized in helping beginners learn programming.",
    AgentName = "Programming Tutor",
  IsCloudMode = true
};
var customAgent = factory.CreateChatAgent(customConfig);
```

### Using in Dependency Injection

```csharp
// In Program.cs
builder.Services.AddSingleton<IChatAgentFactory, ChatAgentFactory>();

// In a service or component
public class MyService
{
private readonly IChatAgentFactory _factory;
    
    public MyService(IChatAgentFactory factory)
    {
     _factory = factory;
 }
    
    public AIAgent CreateCodeReviewer()
    {
        return _factory.CreateChatAgent(AgentType.CodeExpert);
    }
}
```

### Switching Agents in the UI

Users can switch agents through the Settings page:

1. Navigate to Settings (gear icon in sidebar)
2. Scroll to "Agent Selection" section
3. Click on the desired agent card
4. The system will reinitialize with the new agent (if not in mock mode)

### Accessing Agent Information

```csharp
// Get all available agent types
var allTypes = AgentTypeHelper.GetAllAgentTypes();

// Get display information
var displayName = AgentType.CodeExpert.GetDisplayName(); // "Code Expert"
var description = AgentType.CodeExpert.GetFullDescription(); // "Specialized in programming..."
var icon = AgentType.CodeExpert.GetIcon(); // "??"
var fullDesc = AgentType.CodeExpert.GetDescription(); // "Code Expert - Specialized in..."

// Create configuration
var config = new AgentConfiguration { AgentType = AgentType.CodeExpert };
var instructions = config.GetInstructions(); // Get type-specific instructions
var name = config.GetDefaultName(); // Get default name for the type
```

## Adding New Agent Types

To add a new agent type:

### 1. Update the Enum

```csharp
// In Models/AgentType.cs
public enum AgentType
{
    // ...existing types...
    
    [Description("New Agent - Description of the new agent")]
    NewAgent = 5
}
```

### 2. Add Instructions

```csharp
// In Models/AgentConfiguration.cs
public string GetInstructions()
{
    // ...existing code...
    
    return AgentType switch
    {
      // ...existing cases...
        AgentType.NewAgent => "Instructions for the new agent...",
        _ => "You are a helpful AI assistant."
    };
}

public string GetDefaultName()
{
    return AgentType switch
    {
        // ...existing cases...
        AgentType.NewAgent => "New Agent Name",
        _ => "AI Assistant"
    };
}
```

### 3. Add Icon

```csharp
// In Utilities/AgentTypeHelper.cs
public static string GetIcon(this AgentType agentType)
{
    return agentType switch
    {
        // ...existing cases...
        AgentType.NewAgent => "??",
  _ => "??"
    };
}
```

### 4. Test

The new agent type will automatically appear in the Settings UI and be available for selection.

## Integration with ChatService

The ChatService automatically initializes agents based on the selected type:

```csharp
// Initialize available agents from enum
private void InitializeAgents()
{
    foreach (var agentType in AgentTypeHelper.GetAllAgentTypes())
    {
        _agents.Add(new Agent
        {
            Id = agentType.ToString().ToLower(),
 Name = agentType.GetDisplayName(),
       Description = agentType.GetFullDescription(),
            IconEmoji = agentType.GetIcon(),
            IsDefault = agentType == AgentType.SimpleChat,
            AgentType = agentType
        });
    }
}

// Switch to a different agent
await ChatService.SwitchAgentAsync(AgentType.CodeExpert);
```

## Benefits

1. **Extensible**: Easy to add new agent types without modifying UI code
2. **Type-Safe**: Enum-based selection prevents invalid agent types
3. **User-Friendly**: Visual cards in Settings for easy selection
4. **Configurable**: Support for custom instructions and names
5. **Consistent**: Centralized definitions ensure consistency across the application

## Best Practices

1. **Naming**: Use descriptive names that clearly indicate the agent's purpose
2. **Instructions**: Write clear, specific instructions that guide the AI's behavior
3. **Icons**: Choose emojis that visually represent the agent's role
4. **Descriptions**: Provide concise descriptions that help users understand when to use each agent
5. **Testing**: Test each agent type thoroughly with representative queries

## Future Enhancements

- [ ] Agent-specific tools and capabilities
- [ ] Custom agent creation from UI
- [ ] Agent marketplace/library
- [ ] Multi-agent conversations
- [ ] Agent performance metrics
- [ ] Agent fine-tuning options

## Troubleshooting

### Agent Not Switching
- Ensure Mock Mode is disabled
- Check that cloud/local mode is properly configured
- Verify configuration settings in user secrets

### Missing Agent Types
- Ensure all enum values have corresponding entries in AgentConfiguration
- Check that AgentTypeHelper.GetIcon() includes all types
- Verify UI is properly updated after adding new types

### Custom Instructions Not Working
- Confirm CustomInstructions property is set in AgentConfiguration
- Check that factory is using the configuration correctly
- Verify agent is properly reinitialized after changes
