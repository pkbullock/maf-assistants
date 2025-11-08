# Introduction

Library for Microsoft Agent Framework Assistants

## Agents and Features this repo is planning to achieve

- The agent framework relies on agents, so need a collection of agents.

- Data and Storage
    - ✅ Upload and Read files in SharePoint
    - ✅ Organise Files in SharePoint
    - Build a library of ALL my edge short cuts
    - Build a library of hashtags for social media
    - ✅ Microsoft Graph Integration (see [GRAPH_INTEGRATION.md](docs/GRAPH_INTEGRATION.md))
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
    - ✅ RSS Reader for Websites (see [RSS_READER.md](docs/RSS_READER.md))
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

- ✅ Blazor App (MAF.UI)
  - Dark-themed chat interface
  - Mock mode for development
  - Conversation starters and chat history
  - Real-time message updates via SignalR
  - See MAF.UI project for details

## Detailed Documentation

For detailed documentation, see
    - [GRAPH_INTEGRATION.md](docs/GRAPH_INTEGRATION.md)
    - [RSS_READER.md](docs/RSS_READER.md)

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

Enabling Function Calling with Phi-4 requires this change: https://github.com/microsoft/Foundry-Local/tree/main/samples/python/functioncalling