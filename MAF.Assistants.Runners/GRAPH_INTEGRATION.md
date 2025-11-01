# Microsoft Graph Integration

This document describes the Microsoft Graph integration features added to the MAF Assistants framework.

## Overview

The Microsoft Graph integration provides AI agents with the ability to interact with Microsoft 365 services, including SharePoint and Outlook. The implementation is designed to be reusable, secure, and context-aware.

## Features

### 1. Microsoft Graph Authentication
- **Service**: `GraphService` (base class)
- Authenticates using Azure AD with Client Credentials flow
- Configurable via user secrets
- Connection testing capability

### 2. SharePoint Operations
- **Service**: `SharePointService`
- **Capabilities**:
  - List available SharePoint sites
  - Get drives (document libraries) from a site
  - List files in a document library
  - Read file content
  - Write/update files
  - Context-aware file summaries

### 3. Email Operations
- **Service**: `EmailService`
- **Capabilities**:
  - Read emails from user's mailbox
  - Get email summaries
  - Read specific email content
  - Send emails
  - List mail folders

### 4. AI Agent Tools
- **Tools**: `GraphTools` (static class with AI-callable functions)
- Pre-built functions that can be used by AI agents:
  - `ListSharePointSites`: Lists available SharePoint sites
  - `GetSharePointFiles`: Gets files from a document library
  - `ReadSharePointFile`: Reads file content
  - `WriteSharePointFile`: Writes/updates a file
  - `GetRecentEmails`: Gets recent emails
  - `ReadEmail`: Reads specific email content
  - `SendEmail`: Sends an email

### 5. Human-in-the-Loop Confirmation
- **Middleware**: `HumanInTheLoop`
- Automatically prompts for user confirmation before executing sensitive operations
- Default sensitive operations:
  - Sending emails
  - Writing files to SharePoint
- Extensible: can add or remove functions from the sensitive list

### 6. Context Window Management
- Automatic checking of data size against context window limits
- Smart truncation of large result sets
- Token estimation (1 token ≈ 4 characters)

### 7. Configurable Item Limits
- Maximum items per query can be configured
- Prevents overwhelming the AI with too much data
- Can be overridden per-operation

## Configuration

Add the following settings to your user secrets:

```json
{
  "MicrosoftGraphTenantId": "your-tenant-id",
  "MicrosoftGraphClientId": "your-client-id",
  "MicrosoftGraphClientSecret": "your-client-secret",
  "MicrosoftGraphMaxItems": 100
}
```

### Azure AD App Registration

1. Register an application in Azure AD
2. Create a client secret
3. Grant the following Microsoft Graph API permissions (Application permissions):
   - `Sites.Read.All` or `Sites.ReadWrite.All` for SharePoint
   - `Mail.Read` and `Mail.Send` for Email (on behalf of users)
   - `User.Read.All` for user information
4. Grant admin consent for the permissions

## Usage Examples

### Basic Usage in a Chat Runner

```csharp
using MAF.Assistants.Tools;
using MAF.Assistants.Middleware;
using Microsoft.Agents.AI;

// Create AI agent with Graph tools
AIAgent agent = client.CreateAIAgent(
    instructions: "You are a helpful assistant with access to Microsoft 365.",
    name: "Microsoft365Assistant",
    tools: [
        AIFunctionFactory.Create(GraphTools.ListSharePointSites, "list_sharepoint_sites", "Lists available SharePoint sites"),
        AIFunctionFactory.Create(GraphTools.GetRecentEmails, "get_recent_emails", "Gets recent emails"),
        // ... add other tools as needed
    ])
    .AsBuilder()
    .Use(LogFunctionCalling.LogFunctionCallAsync)
    .Use(HumanInTheLoop.ConfirmSensitiveOperationAsync)
    .Build();
```

### Using Services Directly

```csharp
using MAF.Assistants.Services;

// SharePoint
var sharePointService = new SharePointService();
var sites = await sharePointService.GetSitesAsync(maxItems: 10);
var files = await sharePointService.GetFilesAsync(siteId, driveId);
var content = await sharePointService.GetFileContentAsync(siteId, driveId, itemId);

// Email
var emailService = new EmailService();
var emails = await emailService.GetEmailsAsync(userEmail, "Inbox", maxItems: 20);
await emailService.SendEmailAsync(senderEmail, recipients, subject, body);
```

## Running the Sample

The `GraphChatWithTools` runner demonstrates the full integration:

1. Update `Program.cs` to use the Graph chat runner:
```csharp
await AzureClient.StartChat<GraphChatWithTools>();
```

2. Configure your Microsoft Graph credentials in user secrets

3. Run the application and interact with the AI agent:
```
You: List my SharePoint sites
Assistant: [Lists SharePoint sites with IDs]

You: Get files from site <site-id> and drive <drive-id>
Assistant: [Lists files in the document library]

You: Read file <item-id> from that drive
Assistant: [Displays file content]
```

## Security Considerations

1. **Authentication**: Uses secure Client Credentials flow
2. **Secrets Management**: Credentials stored in user secrets, not in code
3. **Human Confirmation**: Sensitive operations require explicit user approval
4. **Least Privilege**: Configure only necessary API permissions
5. **Context Limits**: Automatic checking prevents information overload

## Extensibility

### Adding New Tools

1. Add methods to `GraphTools.cs` with proper descriptions
2. Use `[Description]` attributes for function and parameter documentation
3. Register the tool when creating the AI agent
4. Add to sensitive functions list if needed:
```csharp
HumanInTheLoop.AddSensitiveFunction("your_function_name");
```

### Creating New Services

1. Extend `GraphService` base class
2. Use `GraphClient` property to access Microsoft Graph
3. Use `MaxItems` property for item limits
4. Use `CheckContextWindowSize()` for large data validation

## Architecture

```
Services/
  ├── GraphService.cs          # Base service with authentication
  ├── SharePointService.cs     # SharePoint-specific operations
  └── EmailService.cs          # Email-specific operations

Tools/
  └── GraphTools.cs            # AI-callable function wrappers

Middleware/
  └── HumanInTheLoop.cs        # Confirmation middleware

Utility/
  └── ConfigManager.cs         # Updated with Graph config
```

## Troubleshooting

### Authentication Errors
- Verify Azure AD app registration and permissions
- Check that admin consent has been granted
- Ensure credentials in user secrets are correct

### Permission Errors
- Verify the app has necessary Graph API permissions
- Check that application permissions (not delegated) are used
- Confirm admin consent is granted

### Context Window Warnings
- Reduce `MicrosoftGraphMaxItems` configuration
- Use more specific queries to limit data
- The system will automatically truncate large results

## Future Enhancements

Potential additions to consider:
- Support for delegated permissions (user authentication)
- Additional Microsoft 365 services (Teams, OneDrive, Planner)
- Batch operations for better performance
- Caching layer for frequently accessed data
- More sophisticated context window management
- Support for rich content (images, attachments)
