# Microsoft Graph Integration

This document describes the Microsoft Graph integration features added to the MAF Assistants framework.

## Overview

The Microsoft Graph integration provides AI agents with the ability to interact with Microsoft 365 services, including SharePoint and Outlook. The implementation is designed to be reusable, secure, and context-aware.

The framework now includes comprehensive Microsoft Graph integration for connecting AI agents to Microsoft 365 services:

### Summary Features
- **SharePoint Operations**: List sites, read/write files, manage document libraries
- **Email Operations**: Read emails, send emails, manage mail folders
- **Human-in-the-Loop**: Automatic confirmation prompts for sensitive operations
- **Context Window Management**: Smart handling of large data sets
- **Configurable Limits**: Control how many items are retrieved per operation
- **Reusable Components**: Services and tools that can be easily integrated into any agent

### Quick Start
1. Configure your Azure AD app registration with Microsoft Graph permissions
2. Add credentials to user secrets:
   ```json
   {
     "MicrosoftGraphTenantId": "your-tenant-id",
     "MicrosoftGraphClientId": "your-client-id",
     "MicrosoftGraphClientSecret": "your-client-secret",
     "MicrosoftGraphMaxItems": 100
   }
   ```
3. Use the `GraphChatWithTools` runner or integrate Graph tools into your own agent

## Detailed Features

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
  - **List SharePoint lists** from a site
  - **Get list items** with OData filtering
  - **Convert list items to markdown tables** with field selection

### 3. Email Operations
- **Service**: `EmailService`
- **Capabilities**:
  - Read emails from user's mailbox
  - Get email summaries
  - Read specific email content
  - Send emails
  - Send emails with attachments
  - Send HTML emails with inline images
  - Get and download attachments
  - List mail folders

### 4. Teams Operations
- **Service**: `TeamsService`
- **Capabilities**:
  - List teams user is a member of
  - Get channels in a team
  - Get messages from a channel
  - Send messages to a channel
  - Context-aware message summaries

### 5. OneDrive Operations
- **Service**: `OneDriveService`
- **Capabilities**:
  - List files in OneDrive
  - Get files from specific folders
  - Read text file content
  - Read binary files (images, documents)
  - Upload text files
  - Upload binary files
  - Search files
  - Context-aware file summaries

### 6. Planner Operations
- **Service**: `PlannerService`
- **Capabilities**:
  - List accessible plans
  - Get tasks from a plan
  - Get tasks assigned to user
  - Create new tasks
  - Update task completion status
  - Get buckets from a plan
  - Context-aware task summaries

### 7. Batch Operations
- **Service**: `GraphBatchService`
- **Capabilities**:
  - Batch retrieve multiple file contents
  - Batch retrieve multiple emails
  - Batch retrieve multiple user profiles
  - Batch upload files to OneDrive
  - Up to 20 operations per batch
  - Improved performance for multiple operations

### 8. AI Agent Tools
- **Tools**: `GraphTools` (static class with AI-callable functions)
- Pre-built functions that can be used by AI agents:
  - **SharePoint**: `ListSharePointSites`, `ListSharePointLists`, `GetSharePointFiles`, `GetSharePointListItems`, `ReadSharePointFile`, `WriteSharePointFile`
  - **Email**: `GetRecentEmails`, `ReadEmail`, `SendEmail`
  - **Teams**: `ListTeams`, `GetTeamsChannelMessages`, `SendTeamsMessage`
  - **OneDrive**: `ListOneDriveFiles`, `ReadOneDriveFile`, `UploadToOneDrive`
  - **Planner**: `ListPlannerPlans`, `GetMyPlannerTasks`, `CreatePlannerTask`

### 9. Human-in-the-Loop Confirmation
- **Middleware**: `HumanInTheLoop`
- Automatically prompts for user confirmation before executing sensitive operations
- Default sensitive operations:
  - Sending emails
  - Writing files to SharePoint
  - Sending Teams messages
  - Uploading to OneDrive
  - Creating Planner tasks
- Extensible: can add or remove functions from the sensitive list

### 10. Context Window Management
- Automatic checking of data size against context window limits
- Smart truncation of large result sets
- Token estimation (1 token ≈ 4 characters)

### 11. Configurable Item Limits
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
  "MicrosoftGraphMaxItems": 100,
  "MicrosoftGraphUseDelegatedAuth": false,
  "MicrosoftGraphRedirectUri": "http://localhost"
}
```

### Configuration Options

- `MicrosoftGraphTenantId`: Azure AD tenant ID (required)
- `MicrosoftGraphClientId`: Azure AD application (client) ID (required)
- `MicrosoftGraphClientSecret`: Client secret for application authentication (required for app-only auth)
- `MicrosoftGraphMaxItems`: Default maximum items to retrieve per operation (default: 100)
- `MicrosoftGraphUseDelegatedAuth`: Use delegated (user) authentication instead of application authentication (default: false)
- `MicrosoftGraphRedirectUri`: Redirect URI for delegated authentication (default: "http://localhost")

### Azure AD App Registration

#### For Application Authentication (App-Only)
1. Register an application in Azure AD
2. Create a client secret
3. Grant the following Microsoft Graph API permissions (Application permissions):
   - `Sites.Read.All` or `Sites.ReadWrite.All` for SharePoint (includes sites, lists, and list items)
   - `Mail.Read` and `Mail.Send` for Email
   - `User.Read.All` for user information
   - `Team.ReadBasic.All`, `Channel.ReadBasic.All`, `ChannelMessage.Read.All` for Teams
   - `Files.Read.All`, `Files.ReadWrite.All` for OneDrive
   - `Tasks.Read`, `Tasks.ReadWrite` for Planner
4. Grant admin consent for the permissions

#### For Delegated Authentication (User Context)
1. Register an application in Azure AD
2. Configure redirect URI (e.g., http://localhost)
3. Grant the following Microsoft Graph API permissions (Delegated permissions):
   - `Sites.Read.All` or `Sites.ReadWrite.All` for SharePoint (includes sites, lists, and list items)
   - `Mail.Read` and `Mail.Send` for Email
   - `User.Read` for user information
   - `Team.ReadBasic.All`, `Channel.ReadBasic.All`, `ChannelMessage.Send` for Teams
   - `Files.Read.All`, `Files.ReadWrite.All` for OneDrive
   - `Tasks.Read`, `Tasks.ReadWrite` for Planner
4. Set `MicrosoftGraphUseDelegatedAuth` to `true` in configuration
5. User will be prompted to sign in via browser on first use

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
        AIFunctionFactory.Create(GraphTools.ListSharePointLists, "list_sharepoint_lists", "Lists SharePoint lists from a site"),
        AIFunctionFactory.Create(GraphTools.GetSharePointListItems, "get_sharepoint_list_items", "Gets items from a SharePoint list"),
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

// SharePoint Lists
var lists = await sharePointService.GetListsAsync(siteId, maxItems: 20);
var listItems = await sharePointService.GetListItemsAsync(siteId, listId, maxItems: 50, filter: "fields/Status eq 'Active'");
var markdownTable = sharePointService.ConvertListItemsToMarkdownTable(listItems, new List<string> { "Title", "Status", "DueDate" });
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

You: List SharePoint lists from site <site-id>
Assistant: [Lists available SharePoint lists]

You: Get items from list <list-id> where Status is Active
Assistant: [Displays list items in a markdown table format]
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

## Recent Enhancements

### Delegated Authentication
- ✅ Support for delegated permissions (user authentication) via Interactive Browser
- Configure via `MicrosoftGraphUseDelegatedAuth` and `MicrosoftGraphRedirectUri` settings
- Allows agents to act on behalf of the signed-in user

### Additional Microsoft 365 Services
- ✅ **Teams**: List teams, get channels, send/receive messages
- ✅ **OneDrive**: List/search files, read/write files, binary file support
- ✅ **Planner**: List plans, get tasks, create/update tasks

### Batch Operations
- ✅ GraphBatchService for improved performance
- Supports batch file retrievals, email lookups, and uploads
- Up to 20 operations per batch

### SharePoint Lists Support
- ✅ **List SharePoint lists** from a site with configurable item limits
- ✅ **Get list items** with OData filtering for precise data queries
- ✅ **Markdown table conversion** for easy display of list data
- ✅ **Field selection** to control which columns appear in output
- ✅ Automatic filtering of internal SharePoint fields (@, _)
- ✅ Proper escaping of special characters in markdown output

### Rich Content Support
- ✅ Email attachments (send and receive)
- ✅ Inline images in HTML emails
- ✅ Binary file handling (images, documents)
- ✅ OneDrive file upload/download for any content type

## Future Enhancements

Potential additions to consider:
- Caching layer for frequently accessed data
- More sophisticated context window management
- Full batch API endpoint implementation
- Calendar and meeting operations
- Advanced Teams features (tabs, apps)
