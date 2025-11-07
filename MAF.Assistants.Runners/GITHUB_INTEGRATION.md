# GitHub Integration

This document describes the GitHub integration features added to the MAF Assistants framework.

## Overview

The GitHub integration provides AI agents with the ability to interact with GitHub repositories, including reviewing pull requests, issues, and starred repositories. The implementation is designed to be reusable, secure, and context-aware.

## Features

### 1. GitHub Authentication
- **Service**: `GitHubService`
- Authenticates using Personal Access Token (PAT)
- Configurable via user secrets
- Connection testing capability

### 2. Pull Request Operations
- **Service**: `GitHubService`
- **Capabilities**:
  - Review pull requests for authenticated user
  - Get pull requests for specific repository
  - Get pull requests across all accessible repositories
  - Context-aware PR summaries

### 3. Issue Operations
- **Service**: `GitHubService`
- **Capabilities**:
  - Review issues across repositories
  - Get issues for specific repository
  - Review specific issue by ID
  - Filter issues by state (open, closed, all)
  - Context-aware issue summaries

### 4. Starred Repositories
- **Service**: `GitHubService`
- **Capabilities**:
  - Review GitHub stars for authenticated user
  - Get repository details including description, language, and star count
  - Context-aware starred repository summaries

## Architecture

### Service Layer

The `GitHubService` class provides core GitHub operations:

```csharp
public class GitHubService
{
    // Core Operations
    public async Task<bool> TestConnectionAsync()
    public async Task<List<PullRequest>> ReviewPullRequestsAsync(string? owner = null, string? repository = null, int? maxItems = null)
    public async Task<List<Repository>> ReviewGitHubStarsAsync(int? maxItems = null)
    public async Task<List<Issue>> ReviewIssuesAsync(string? owner = null, string? repository = null, ItemStateFilter state = ItemStateFilter.Open, int? maxItems = null)
    public async Task<Issue> ReviewIssueByIdAsync(string owner, string repository, int issueNumber)
    
    // Summary Operations (Context-Aware)
    public async Task<string> GetPullRequestsSummaryAsync(string? owner = null, string? repository = null, int? maxItems = null)
    public async Task<string> GetStarredRepositoriesSummaryAsync(int? maxItems = null)
    public async Task<string> GetIssuesSummaryAsync(string? owner = null, string? repository = null, ItemStateFilter state = ItemStateFilter.Open, int? maxItems = null)
}
```

### Configuration

GitHub integration requires the following configuration in user secrets:

```json
{
  "GitHubToken": "your-github-personal-access-token",
  "GitHubMaxItems": 50
}
```

### Configuration Parameters

- **GitHubToken**: Personal Access Token for GitHub authentication
  - Required permissions: `repo`, `read:user`, `read:org` (depending on your use case)
- **GitHubMaxItems**: Default maximum number of items to retrieve per operation
  - Default: 50
  - Can be overridden per method call

## Setup

### 1. Create GitHub Personal Access Token

1. Go to GitHub Settings > Developer settings > Personal access tokens > Tokens (classic)
2. Click "Generate new token (classic)"
3. Give your token a descriptive name (e.g., "MAF Assistants")
4. Select the required scopes:
   - `repo` - Full control of private repositories (for PRs and issues)
   - `read:user` - Read user profile data
   - `read:org` - Read org and team membership (if accessing org repositories)
5. Click "Generate token"
6. Copy the token (you won't be able to see it again)

### 2. Configure User Secrets

Add the GitHub configuration to your user secrets:

```bash
dotnet user-secrets set "GitHubToken" "your-github-personal-access-token"
dotnet user-secrets set "GitHubMaxItems" "50"
```

Or edit the secrets file directly:

```bash
dotnet user-secrets list
# Use the path shown to edit secrets.json
```

Add:

```json
{
  "GitHubToken": "your-github-personal-access-token",
  "GitHubMaxItems": 50
}
```

## Usage Examples

### Basic Service Usage

```csharp
using MAF.Assistants.Services;

// Initialize the service
var githubService = new GitHubService();

// Test connection
bool isConnected = await githubService.TestConnectionAsync();

// Review pull requests for a specific repository
var prs = await githubService.ReviewPullRequestsAsync("owner", "repository");

// Review all issues for current user
var issues = await githubService.ReviewIssuesAsync();

// Get specific issue
var issue = await githubService.ReviewIssueByIdAsync("owner", "repository", 123);

// Review starred repositories
var stars = await githubService.ReviewGitHubStarsAsync();
```

### Getting Summaries

For context-aware summaries that fit within AI context windows:

```csharp
// Get PR summary
string prSummary = await githubService.GetPullRequestsSummaryAsync("owner", "repository");

// Get issues summary
string issuesSummary = await githubService.GetIssuesSummaryAsync("owner", "repository");

// Get starred repositories summary
string starsSummary = await githubService.GetStarredRepositoriesSummaryAsync();
```

## Context Window Management

The GitHub service includes context window size checking to ensure data fits within AI model constraints:

- **Automatic Truncation**: If summaries exceed estimated token limits, they are automatically truncated
- **Smart Summarization**: Truncated summaries show the first 10 items instead of full details
- **Token Estimation**: Uses a rough estimation of 1 token ≈ 4 characters

## Security Considerations

1. **Token Storage**: Never commit your GitHub token to source control
2. **Token Permissions**: Use the minimum required scopes for your use case
3. **Token Rotation**: Regularly rotate your Personal Access Tokens
4. **User Secrets**: Store tokens in user secrets, environment variables, or Azure Key Vault

## Testing

Unit tests are provided in `MAF.Tests/Services/GitHubServiceTests.cs`:

```bash
dotnet test --filter "FullyQualifiedName~GitHubServiceTests"
```

Tests verify:
- Service instantiation
- Method signatures
- Protected property access

## Dependencies

- **Octokit**: Official GitHub API client for .NET
  - Version: 14.0.0 or higher
  - NuGet: `Octokit`

## Error Handling

The service handles common errors:

- **Missing Configuration**: Throws `InvalidOperationException` if GitHubToken is not configured
- **API Errors**: Octokit exceptions are propagated to the caller
- **Connection Failures**: `TestConnectionAsync()` returns `false` instead of throwing

## Future Enhancements

Potential future additions:

- Create issues and pull requests
- Add comments to issues and PRs
- Repository management (create, update, delete)
- Branch and commit operations
- GitHub Actions workflow management
- Webhook integration
- Organization and team management

## Troubleshooting

### "GitHub configuration is missing" Error

**Problem**: Service throws exception on initialization

**Solution**: Ensure GitHubToken is configured in user secrets:
```bash
dotnet user-secrets set "GitHubToken" "your-token"
```

### API Rate Limiting

**Problem**: GitHub API returns 403 rate limit errors

**Solution**: 
- Authenticated requests have higher rate limits (5000/hour vs 60/hour)
- Wait for rate limit to reset
- Reduce `GitHubMaxItems` to make fewer requests

### Insufficient Permissions

**Problem**: API returns 403/404 for certain operations

**Solution**: 
- Verify your token has the required scopes
- Regenerate token with appropriate permissions
- Check repository access (private vs public)

## Related Documentation

- [Octokit.NET Documentation](https://octokitnet.readthedocs.io/)
- [GitHub REST API Documentation](https://docs.github.com/en/rest)
- [GitHub Personal Access Tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)
