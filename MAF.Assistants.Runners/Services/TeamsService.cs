using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Services
{
    /// <summary>
    /// Service for Microsoft Teams operations via Microsoft Graph.
    /// </summary>
    public class TeamsService : GraphService
    {
        /// <summary>
        /// Gets all teams the user is a member of.
        /// </summary>
        /// <param name="maxItems">Maximum number of teams to retrieve (optional, uses config default if not specified).</param>
        /// <returns>List of teams.</returns>
        public async Task<List<Team>> GetTeamsAsync(int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var teams = await GraphClient.Me.JoinedTeams.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = itemsToRetrieve;
            });

            return teams?.Value?.ToList() ?? new List<Team>();
        }

        /// <summary>
        /// Gets channels in a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <returns>List of channels.</returns>
        public async Task<List<Channel>> GetChannelsAsync(string teamId)
        {
            var channels = await GraphClient.Teams[teamId].Channels.GetAsync();
            return channels?.Value?.ToList() ?? new List<Channel>();
        }

        /// <summary>
        /// Gets messages from a channel.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="maxItems">Maximum number of messages to retrieve.</param>
        /// <returns>List of chat messages.</returns>
        public async Task<List<ChatMessage>> GetChannelMessagesAsync(string teamId, string channelId, int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var messages = await GraphClient.Teams[teamId].Channels[channelId].Messages.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = itemsToRetrieve;
                requestConfiguration.QueryParameters.Orderby = new[] { "createdDateTime DESC" };
            });

            return messages?.Value?.ToList() ?? new List<ChatMessage>();
        }

        /// <summary>
        /// Sends a message to a Teams channel.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="message">The message content.</param>
        /// <returns>The created chat message.</returns>
        public async Task<ChatMessage?> SendChannelMessageAsync(string teamId, string channelId, string message)
        {
            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    Content = message,
                    ContentType = BodyType.Text
                }
            };

            return await GraphClient.Teams[teamId].Channels[channelId].Messages.PostAsync(chatMessage);
        }

        /// <summary>
        /// Gets a summary of teams that fits within context window constraints.
        /// </summary>
        /// <param name="maxItems">Maximum number of teams to retrieve.</param>
        /// <returns>A formatted string summary of teams.</returns>
        public async Task<string> GetTeamsSummaryAsync(int? maxItems = null)
        {
            var teams = await GetTeamsAsync(maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {teams.Count} teams:");

            foreach (var team in teams)
            {
                summary.AppendLine($"- Name: {team.DisplayName}, ID: {team.Id}, Description: {team.Description}");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many teams ({teams.Count}) to display full summary. Showing first 10:");
                foreach (var team in teams.Take(10))
                {
                    summary.AppendLine($"- Name: {team.DisplayName}, ID: {team.Id}");
                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Gets a summary of messages from a channel.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="maxItems">Maximum number of messages to retrieve.</param>
        /// <returns>A formatted string summary of messages.</returns>
        public async Task<string> GetChannelMessagesSummaryAsync(string teamId, string channelId, int? maxItems = null)
        {
            var messages = await GetChannelMessagesAsync(teamId, channelId, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {messages.Count} messages in channel:");

            foreach (var message in messages)
            {
                var from = message.From?.User?.DisplayName ?? "Unknown";
                var content = message.Body?.Content ?? "(No Content)";
                var created = message.CreatedDateTime?.ToString("yyyy-MM-dd HH:mm") ?? "Unknown";
                
                // Truncate long messages
                if (content.Length > 100)
                    content = content.Substring(0, 100) + "...";
                
                summary.AppendLine($"- From: {from}, Created: {created}, Message: {content}");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many messages ({messages.Count}) to display full summary. Showing first 10:");
                foreach (var message in messages.Take(10))
                {
                    var from = message.From?.User?.DisplayName ?? "Unknown";
                    var created = message.CreatedDateTime?.ToString("yyyy-MM-dd HH:mm") ?? "Unknown";
                    summary.AppendLine($"- From: {from}, Created: {created}");
                }
            }

            return summary.ToString();
        }
    }
}
