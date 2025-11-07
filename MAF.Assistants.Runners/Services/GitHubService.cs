using MAF.Assistants.Utility;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Services
{
    /// <summary>
    /// Service for GitHub operations via Octokit.
    /// Provides access to pull requests, issues, and starred repositories.
    /// </summary>
    public class GitHubService
    {
        private readonly GitHubClient _githubClient;
        private readonly int _maxItems;

        /// <summary>
        /// Initializes a new instance of the GitHubService with authentication.
        /// Uses Personal Access Token (PAT) for authentication.
        /// </summary>
        public GitHubService()
        {
            var config = ConfigManager.GetConfig();
            _maxItems = config.GitHubMaxItems;

            if (string.IsNullOrEmpty(config.GitHubToken))
            {
                throw new InvalidOperationException("GitHub configuration is missing. Please configure GitHubToken.");
            }

            _githubClient = new GitHubClient(new ProductHeaderValue("MAF-Assistants"));
            var tokenAuth = new Credentials(config.GitHubToken);
            _githubClient.Credentials = tokenAuth;
        }

        /// <summary>
        /// Gets the configured GitHubClient instance.
        /// </summary>
        protected GitHubClient GitHubClient => _githubClient;

        /// <summary>
        /// Gets the maximum number of items to retrieve in a single operation.
        /// </summary>
        protected int MaxItems => _maxItems;

        /// <summary>
        /// Tests the connection to GitHub by retrieving the authenticated user details.
        /// </summary>
        /// <returns>True if connection is successful, false otherwise.</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var user = await _githubClient.User.Current();
                return user != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Reviews pull requests for the authenticated user.
        /// Gets all open pull requests across repositories the user has access to.
        /// </summary>
        /// <param name="owner">Repository owner (optional). If not provided, gets PRs for current user.</param>
        /// <param name="repository">Repository name (optional). If provided, owner must also be provided.</param>
        /// <param name="maxItems">Maximum number of pull requests to retrieve.</param>
        /// <returns>List of pull requests.</returns>
        public async Task<List<PullRequest>> ReviewPullRequestsAsync(string? owner = null, string? repository = null, int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            var pullRequests = new List<PullRequest>();

            if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repository))
            {
                // Get PRs for specific repository
                var request = new PullRequestRequest
                {
                    State = ItemStateFilter.Open
                };
                
                var prs = await _githubClient.PullRequest.GetAllForRepository(owner, repository, request);
                pullRequests.AddRange(prs.Take(itemsToRetrieve));
            }
            else
            {
                // Get PRs across all repositories for the current user
                var searchRequest = new SearchIssuesRequest
                {
                    Type = IssueTypeQualifier.PullRequest,
                    State = ItemState.Open
                };

                var currentUser = await _githubClient.User.Current();
                searchRequest.Involves = currentUser.Login;

                var searchResult = await _githubClient.Search.SearchIssues(searchRequest);
                
                // Convert search results to pull requests
                foreach (var issue in searchResult.Items.Take(itemsToRetrieve))
                {
                    var parts = issue.Url.ToString().Split('/');
                    var repoOwner = parts[^4];
                    var repoName = parts[^3];
                    var prNumber = issue.Number;
                    
                    var pr = await _githubClient.PullRequest.Get(repoOwner, repoName, prNumber);
                    pullRequests.Add(pr);
                }
            }

            return pullRequests;
        }

        /// <summary>
        /// Reviews GitHub stars for the authenticated user.
        /// Gets repositories that the user has starred.
        /// </summary>
        /// <param name="maxItems">Maximum number of starred repositories to retrieve.</param>
        /// <returns>List of starred repositories.</returns>
        public async Task<List<Repository>> ReviewGitHubStarsAsync(int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var starredRepos = await _githubClient.Activity.Starring.GetAllForCurrent();
            return starredRepos.Take(itemsToRetrieve).ToList();
        }

        /// <summary>
        /// Reviews issues across repositories the authenticated user has access to.
        /// </summary>
        /// <param name="owner">Repository owner (optional). If not provided, gets issues across all repos.</param>
        /// <param name="repository">Repository name (optional). If provided, owner must also be provided.</param>
        /// <param name="state">Issue state filter (open, closed, all). Defaults to open.</param>
        /// <param name="maxItems">Maximum number of issues to retrieve.</param>
        /// <returns>List of issues.</returns>
        public async Task<List<Issue>> ReviewIssuesAsync(string? owner = null, string? repository = null, ItemStateFilter state = ItemStateFilter.Open, int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repository))
            {
                // Get issues for specific repository
                var request = new RepositoryIssueRequest
                {
                    State = state
                };
                
                var issues = await _githubClient.Issue.GetAllForRepository(owner, repository, request);
                return issues.Take(itemsToRetrieve).ToList();
            }
            else
            {
                // Get issues across all repositories for the current user
                var request = new IssueRequest
                {
                    Filter = IssueFilter.All,
                    State = state
                };
                
                var issues = await _githubClient.Issue.GetAllForCurrent(request);
                return issues.Take(itemsToRetrieve).ToList();
            }
        }

        /// <summary>
        /// Reviews a specific issue by ID.
        /// </summary>
        /// <param name="owner">Repository owner.</param>
        /// <param name="repository">Repository name.</param>
        /// <param name="issueNumber">Issue number.</param>
        /// <returns>The issue details.</returns>
        public async Task<Issue> ReviewIssueByIdAsync(string owner, string repository, int issueNumber)
        {
            return await _githubClient.Issue.Get(owner, repository, issueNumber);
        }

        /// <summary>
        /// Gets a summary of pull requests.
        /// </summary>
        /// <param name="owner">Repository owner (optional).</param>
        /// <param name="repository">Repository name (optional).</param>
        /// <param name="maxItems">Maximum number of pull requests to retrieve.</param>
        /// <returns>A formatted string summary of pull requests.</returns>
        public async Task<string> GetPullRequestsSummaryAsync(string? owner = null, string? repository = null, int? maxItems = null)
        {
            var pullRequests = await ReviewPullRequestsAsync(owner, repository, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {pullRequests.Count} pull request(s):");

            foreach (var pr in pullRequests)
            {
                summary.AppendLine($"- #{pr.Number}: {pr.Title}");
                summary.AppendLine($"  Author: {pr.User.Login}, State: {pr.State}, Created: {pr.CreatedAt:yyyy-MM-dd}");
                summary.AppendLine($"  URL: {pr.HtmlUrl}");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many pull requests ({pullRequests.Count}) to display full summary. Showing first 10:");
                foreach (var pr in pullRequests.Take(10))
                {
                    summary.AppendLine($"- #{pr.Number}: {pr.Title} by {pr.User.Login}");
                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Gets a summary of starred repositories.
        /// </summary>
        /// <param name="maxItems">Maximum number of starred repositories to retrieve.</param>
        /// <returns>A formatted string summary of starred repositories.</returns>
        public async Task<string> GetStarredRepositoriesSummaryAsync(int? maxItems = null)
        {
            var starredRepos = await ReviewGitHubStarsAsync(maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {starredRepos.Count} starred repository(ies):");

            foreach (var repo in starredRepos)
            {
                summary.AppendLine($"- {repo.FullName}");
                summary.AppendLine($"  Description: {repo.Description ?? "No description"}");
                summary.AppendLine($"  Stars: {repo.StargazersCount}, Language: {repo.Language ?? "N/A"}");
                summary.AppendLine($"  URL: {repo.HtmlUrl}");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many starred repositories ({starredRepos.Count}) to display full summary. Showing first 10:");
                foreach (var repo in starredRepos.Take(10))
                {
                    summary.AppendLine($"- {repo.FullName}: {repo.Description ?? "No description"}");
                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Gets a summary of issues.
        /// </summary>
        /// <param name="owner">Repository owner (optional).</param>
        /// <param name="repository">Repository name (optional).</param>
        /// <param name="state">Issue state filter.</param>
        /// <param name="maxItems">Maximum number of issues to retrieve.</param>
        /// <returns>A formatted string summary of issues.</returns>
        public async Task<string> GetIssuesSummaryAsync(string? owner = null, string? repository = null, ItemStateFilter state = ItemStateFilter.Open, int? maxItems = null)
        {
            var issues = await ReviewIssuesAsync(owner, repository, state, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {issues.Count} issue(s):");

            foreach (var issue in issues)
            {
                summary.AppendLine($"- #{issue.Number}: {issue.Title}");
                summary.AppendLine($"  Author: {issue.User.Login}, State: {issue.State}, Created: {issue.CreatedAt:yyyy-MM-dd}");
                summary.AppendLine($"  URL: {issue.HtmlUrl}");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many issues ({issues.Count}) to display full summary. Showing first 10:");
                foreach (var issue in issues.Take(10))
                {
                    summary.AppendLine($"- #{issue.Number}: {issue.Title} by {issue.User.Login}");
                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Estimates if the data size would fit within a context window.
        /// </summary>
        /// <param name="textContent">The text content to check.</param>
        /// <param name="maxTokens">Maximum tokens allowed (default 8000).</param>
        /// <returns>True if estimated to fit, false otherwise.</returns>
        protected bool CheckContextWindowSize(string textContent, int maxTokens = 8000)
        {
            // Rough estimation: 1 token ≈ 4 characters
            int estimatedTokens = textContent.Length / 4;
            return estimatedTokens <= maxTokens;
        }
    }
}
