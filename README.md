# Introduction

Library for Microsoft Agent Framework Assistants

## Agents and Features this repo is planning to achieve

- The agent framework relies on agents, so need a collection of agents.

- Data and Storage
    - ✅ Upload and Read files in SharePoint
    - ✅ Organise Files in SharePoint
    - Build a library of ALL my edge short cuts
    - Build a library of hashtags for social media
    - ✅ Microsoft Graph Integration (see [GRAPH_INTEGRATION.md](MAF.Assistants.Runners/GRAPH_INTEGRATION.md))
- Process
    - Trigger Copilot Studio Agent
    - Trigger AI Foundry Agent
    - Invoke a Logic App (keeps the agent flexible)
    - ✅ Invoke a local GPT model (offline fallback)
    - Invoke a local Whisper model (NPU)
    - Invoke a local vision model to read document page
    - Shorten Text for Social Media
- Utility/Tool
    - Get Date
    - ✅ Authenticate to Microsoft 365 (Application)
    - ✅ Read and Send Emails via Microsoft Graph
    - ✅ RSS Reader for Websites (see [RSS_READER.md](MAF.Assistants.Runners/RSS_READER.md))
    - Opens a URL in Edge
- Agent to Agent (Microsoft 365 SDK)
    - Handoff to Copilot Studio Agent
    - Handoff to Azure AI Foundry Agent

## Agent Foundation Capabilities

- Storing state data
- Memory
- Conversation History
- Embeddings and Vectorisation

## Build a front end for MAF Chat

- Blazor App

## Microsoft Graph Integration

The framework now includes comprehensive Microsoft Graph integration for connecting AI agents to Microsoft 365 services:

### Features
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

For detailed documentation, see [GRAPH_INTEGRATION.md](MAF.Assistants.Runners/GRAPH_INTEGRATION.md)

## RSS Reader Service

The framework includes an RSS Reader service for reading and parsing RSS/Atom feeds from websites:

### Features
- **Feed Parsing**: Read RSS 2.0 and Atom feeds from any URL
- **Structured Data**: Extract title, published date, content, excerpt, author, and links
- **URL Extraction**: Automatically extract referenced URLs from content
- **HTML Processing**: Strip HTML tags and decode entities for clean text
- **Flexible Retrieval**: Read all items or limit to a specific number

### Quick Start
1. Create an instance of `RssReaderService`
2. Call `ReadFeedAsync` with the feed URL:
   ```csharp
   var rssService = new RssReaderService();
   var items = await rssService.ReadFeedAsync("https://example.com/feed.xml", maxItems: 10);
   
   foreach (var item in items)
   {
       Console.WriteLine($"{item.Title} - {item.Link}");
       Console.WriteLine($"Published: {item.PublishedDate}");
       Console.WriteLine($"Excerpt: {item.Excerpt}");
   }
   ```

For detailed documentation, see [RSS_READER.md](MAF.Assistants.Runners/RSS_READER.md)

## Useful Resources

- Microsoft Agent Framework Documentation - https://learn.microsoft.com/en-us/ai/agent-framework/
- Microsoft 365 Agents SDK for dotnet - https://github.com/microsoft/Agents-for-net
- Microsoft 365 Agents SDK - https://github.com/microsoft/Agents
- Microsoft Copilot Studio - https://learn.microsoft.com/en-us/microsoft-copilot-studio/fundamentals-what-is-copilot-studio?WT.mc_id=M365-MVP-5003816
- Microsoft 365 Agents SDK documentation - https://learn.microsoft.com/en-us/microsoft-365/agents-sdk/?WT.mc_id=M365-MVP-5003816
- Microsoft Graph API - https://learn.microsoft.com/en-us/graph/overview
- Microsoft Graph SDK for .NET - https://learn.microsoft.com/en-us/graph/sdks/sdks-overview
- Windows-DevRel - https://github.com/microsoft/Windows-DevRel
- Context-Aware RAG and Chunking - https://techcommunity.microsoft.com/blog/azure-ai-foundry-blog/context-aware-rag-system-with-azure-ai-search-to-cut-token-costs-and-boost-accur/4456810
- Hugging Face  - all-MiniLM-L6-v2 - https://huggingface.co/optimum/all-MiniLM-L6-v2


## Local Models

The Phi-3 model range does not support tool calling. 

- "FoundryLocalChatDeploymentName": "phi-3.5-mini-128k-instruct-qnn-npu:2",
- "FoundryLocalChatDeploymentName": "Phi-4-mini-instruct-generic-cpu:4",

Enabling Functiona Calling with Phi-4 requires this change: https://github.com/microsoft/Foundry-Local/tree/main/samples/python/functioncalling