using Azure.Identity;
using MAF.Assistants.Utility;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Threading.Tasks;

namespace MAF.Assistants.Services
{
    /// <summary>
    /// Base service for Microsoft Graph operations with authentication and connection management.
    /// </summary>
    public class GraphService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly int _maxItems;

        /// <summary>
        /// Initializes a new instance of the GraphService with authentication.
        /// </summary>
        public GraphService()
        {
            var config = ConfigManager.GetConfig();
            _maxItems = config.MicrosoftGraphMaxItems;

            if (string.IsNullOrEmpty(config.MicrosoftGraphTenantId) ||
                string.IsNullOrEmpty(config.MicrosoftGraphClientId) ||
                string.IsNullOrEmpty(config.MicrosoftGraphClientSecret))
            {
                throw new InvalidOperationException("Microsoft Graph configuration is missing. Please configure MicrosoftGraphTenantId, MicrosoftGraphClientId, and MicrosoftGraphClientSecret.");
            }

            var clientSecretCredential = new ClientSecretCredential(
                config.MicrosoftGraphTenantId,
                config.MicrosoftGraphClientId,
                config.MicrosoftGraphClientSecret);

            _graphClient = new GraphServiceClient(clientSecretCredential);
        }

        /// <summary>
        /// Gets the configured GraphServiceClient instance.
        /// </summary>
        protected GraphServiceClient GraphClient => _graphClient;

        /// <summary>
        /// Gets the maximum number of items to retrieve in a single operation.
        /// </summary>
        protected int MaxItems => _maxItems;

        /// <summary>
        /// Tests the connection to Microsoft Graph by retrieving the organization details.
        /// </summary>
        /// <returns>True if connection is successful, false otherwise.</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var organization = await _graphClient.Organization.GetAsync();
                return organization?.Value?.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
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
