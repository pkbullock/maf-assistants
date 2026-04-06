# GitHub Copilot Instructions for MAF Assistants

## Project Overview

This repository contains **MAF Assistants**, a library built on the Microsoft Agents Framework (MAF) that provides AI agent capabilities using Azure OpenAI and local Foundry models. The solution consists of three projects:

- **MAF.Assistants.Runners** – Core agent library (agents, runners, tools, services)
- **MAF.UI** – Blazor Server frontend
- **MAF.Tests** – xUnit test suite

## Microsoft Agents Framework Version Updates

### How to Check for Updates

To check for available NuGet updates:

```bash
dotnet list MAF.Assistants.Runners/MAF.Assistants.csproj package --outdated
```

### Current Key Package

The Microsoft Agents Framework GA package is referenced in `MAF.Assistants.Runners/MAF.Assistants.csproj`:

```xml
<PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0" />
```

### Updating the Microsoft Agents Framework

1. Update the package version in `MAF.Assistants.Runners/MAF.Assistants.csproj`:
   ```bash
   dotnet add MAF.Assistants.Runners/MAF.Assistants.csproj package Microsoft.Agents.AI.OpenAI --version <new-version>
   ```

2. Build the solution to check for breaking changes:
   ```bash
   dotnet build MAF.Assistants.Runners/MAF.Assistants.sln
   ```

3. Run the test suite to validate changes:
   ```bash
   dotnet test MAF.Assistants.Runners/MAF.Assistants.sln
   ```

### GA vs Preview API Differences

The Microsoft Agents Framework GA release (`1.0.0`) introduced several API changes from the preview versions:

| Preview API | GA API | Notes |
|---|---|---|
| `client.CreateAIAgent(instructions, name)` | `client.AsAIAgent(instructions, name)` | Extension method renamed |
| `AgentThread` | `AgentSession` | Type renamed |
| `agent.GetNewThread()` | `await agent.CreateSessionAsync(default)` | Method renamed and made async |
| `thread.Serialize(options)` | `await agent.SerializeSessionAsync(session, options)` | Moved to agent method |
| `agent.DeserializeThread(element, options)` | `await agent.DeserializeSessionAsync(element, options)` | Method renamed |
| `agent.RunStreamingAsync(prompt)` | `agent.RunStreamingAsync(prompt, session)` | Session is now required |
| `agent.RunAsync(prompt)` | `agent.RunAsync(prompt, session)` | Session is now required |

### Key Pattern Changes

When updating from preview to GA, ensure:

1. **Agent creation** – Replace `CreateAIAgent` with `AsAIAgent` in all agent classes under `MAF.Assistants.Runners/Agents/` and runner classes under `MAF.Assistants.Runners/Runners/`.

2. **Session management** – All `RunAsync` and `RunStreamingAsync` calls require an `AgentSession`. Create a session using:
   ```csharp
   AgentSession session = await agent.CreateSessionAsync(default);
   ```
   For long-lived services (e.g., `AgentService`), store the session as a field and reuse it across calls to maintain conversation history.

3. **Persistence** – The `Persistence` utility in `MAF.Assistants.Runners/Utility/Persistence.cs` handles saving/loading agent sessions. The `SaveConversationToFileAsync` method takes both an `AIAgent` and `AgentSession`.

### Files to Update When the Framework Changes

- `MAF.Assistants.Runners/MAF.Assistants.csproj` – Package version
- `MAF.Assistants.Runners/Agents/*.cs` – All agent implementations (7 files)
- `MAF.Assistants.Runners/Runners/*.cs` – All runner implementations (4 files)
- `MAF.Assistants.Runners/Utility/Persistence.cs` – Session serialization
- `MAF.UI/Services/AgentService.cs` – UI agent service
- `MAF.Tests/Utility/PersistenceTests.cs` – Tests for Persistence utility

## Building and Testing

```bash
# Build the solution
dotnet build MAF.Assistants.Runners/MAF.Assistants.sln

# Run tests (excludes live integration tests that require credentials)
dotnet test MAF.Assistants.Runners/MAF.Assistants.sln

# Check for outdated packages
dotnet list MAF.Assistants.Runners/MAF.Assistants.csproj package --outdated
```

## Project Conventions

- **Agent classes** implement `IChatAgent` with a static `DefaultConfiguration` property
- **Client creation** uses `Clients.GetAzureChat()` for cloud or `Clients.GetLocalFoundryChat()` for local models
- **Streaming** is preferred over non-streaming for UI interactions
- **Session** must be created before any `RunAsync`/`RunStreamingAsync` call and reused for the conversation
- **Tools** are registered via `AIFunctionFactory.Create()` and passed to `AsAIAgent()`
- **Middleware** (logging, human-in-the-loop) is applied via `.AsBuilder().Use(...).Build()`
