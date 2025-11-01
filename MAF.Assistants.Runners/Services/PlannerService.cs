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
    /// Service for Microsoft Planner operations via Microsoft Graph.
    /// </summary>
    public class PlannerService : GraphService
    {
        /// <summary>
        /// Gets all plans accessible to the user.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="maxItems">Maximum number of plans to retrieve (optional, uses config default if not specified).</param>
        /// <returns>List of planner plans.</returns>
        public async Task<List<PlannerPlan>> GetPlansAsync(string userId = "me", int? maxItems = null)
        {
            var itemsToRetrieve = maxItems ?? MaxItems;
            
            var plans = await GraphClient.Users[userId].Planner.Plans.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = itemsToRetrieve;
            });

            return plans?.Value?.ToList() ?? new List<PlannerPlan>();
        }

        /// <summary>
        /// Gets tasks from a specific plan.
        /// </summary>
        /// <param name="planId">The plan ID.</param>
        /// <returns>List of planner tasks.</returns>
        public async Task<List<PlannerTask>> GetTasksAsync(string planId)
        {
            var tasks = await GraphClient.Planner.Plans[planId].Tasks.GetAsync();
            return tasks?.Value?.ToList() ?? new List<PlannerTask>();
        }

        /// <summary>
        /// Gets tasks assigned to a user.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <returns>List of planner tasks.</returns>
        public async Task<List<PlannerTask>> GetUserTasksAsync(string userId = "me")
        {
            var tasks = await GraphClient.Users[userId].Planner.Tasks.GetAsync();
            return tasks?.Value?.ToList() ?? new List<PlannerTask>();
        }

        /// <summary>
        /// Creates a new task in a plan.
        /// </summary>
        /// <param name="planId">The plan ID.</param>
        /// <param name="bucketId">The bucket ID.</param>
        /// <param name="title">The task title.</param>
        /// <param name="dueDateTime">Optional due date.</param>
        /// <returns>The created planner task.</returns>
        public async Task<PlannerTask?> CreateTaskAsync(string planId, string bucketId, string title, DateTimeOffset? dueDateTime = null)
        {
            var task = new PlannerTask
            {
                PlanId = planId,
                BucketId = bucketId,
                Title = title,
                DueDateTime = dueDateTime
            };

            return await GraphClient.Planner.Tasks.PostAsync(task);
        }

        /// <summary>
        /// Updates a task's completion status.
        /// </summary>
        /// <param name="taskId">The task ID.</param>
        /// <param name="isComplete">Whether the task is complete.</param>
        /// <param name="percentComplete">Percentage complete (0-100).</param>
        /// <returns>The updated planner task.</returns>
        public async Task<PlannerTask?> UpdateTaskAsync(string taskId, bool isComplete, int percentComplete)
        {
            // First get the task to get the ETag
            var existingTask = await GraphClient.Planner.Tasks[taskId].GetAsync();
            if (existingTask == null || existingTask.AdditionalData == null)
                return null;

            var task = new PlannerTask
            {
                PercentComplete = percentComplete
            };

            // Get ETag from response headers
            string? etag = null;
            if (existingTask.AdditionalData.TryGetValue("@odata.etag", out var etagObj))
            {
                etag = etagObj?.ToString();
            }

            if (string.IsNullOrEmpty(etag))
                return null;

            return await GraphClient.Planner.Tasks[taskId].PatchAsync(task, requestConfiguration =>
            {
                requestConfiguration.Headers.Add("If-Match", etag);
            });
        }

        /// <summary>
        /// Gets buckets (task containers) from a plan.
        /// </summary>
        /// <param name="planId">The plan ID.</param>
        /// <returns>List of planner buckets.</returns>
        public async Task<List<PlannerBucket>> GetBucketsAsync(string planId)
        {
            var buckets = await GraphClient.Planner.Plans[planId].Buckets.GetAsync();
            return buckets?.Value?.ToList() ?? new List<PlannerBucket>();
        }

        /// <summary>
        /// Gets a summary of plans that fits within context window constraints.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <param name="maxItems">Maximum number of plans to retrieve.</param>
        /// <returns>A formatted string summary of plans.</returns>
        public async Task<string> GetPlansSummaryAsync(string userId = "me", int? maxItems = null)
        {
            var plans = await GetPlansAsync(userId, maxItems);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {plans.Count} plans:");

            foreach (var plan in plans)
            {
                summary.AppendLine($"- Title: {plan.Title}, ID: {plan.Id}, Owner: {plan.Owner}");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many plans ({plans.Count}) to display full summary. Showing first 10:");
                foreach (var plan in plans.Take(10))
                {
                    summary.AppendLine($"- Title: {plan.Title}, ID: {plan.Id}");
                }
            }

            return summary.ToString();
        }

        /// <summary>
        /// Gets a summary of user tasks.
        /// </summary>
        /// <param name="userId">The user ID (use "me" for current user in delegated auth).</param>
        /// <returns>A formatted string summary of tasks.</returns>
        public async Task<string> GetUserTasksSummaryAsync(string userId = "me")
        {
            var tasks = await GetUserTasksAsync(userId);
            var summary = new StringBuilder();
            summary.AppendLine($"Found {tasks.Count} tasks assigned to user:");

            foreach (var task in tasks)
            {
                var status = task.PercentComplete == 100 ? "✓ Complete" : $"{task.PercentComplete}% Complete";
                var due = task.DueDateTime?.ToString("yyyy-MM-dd") ?? "No due date";
                summary.AppendLine($"- {task.Title} ({status}, Due: {due})");
            }

            var summaryText = summary.ToString();
            if (!CheckContextWindowSize(summaryText))
            {
                summary.Clear();
                summary.AppendLine($"Warning: Too many tasks ({tasks.Count}) to display full summary. Showing first 20:");
                foreach (var task in tasks.Take(20))
                {
                    var status = task.PercentComplete == 100 ? "✓" : $"{task.PercentComplete}%";
                    summary.AppendLine($"- {task.Title} ({status})");
                }
            }

            return summary.ToString();
        }
    }
}
