using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MAF.Assistants.Middleware
{
    /// <summary>
    /// Middleware that requires human confirmation before executing certain sensitive operations.
    /// </summary>
    public class HumanInTheLoop
    {
        private static readonly HashSet<string> SensitiveFunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SendEmail",
            "WriteSharePointFile",
            "send_email",
            "write_sharepoint_file"
        };

        /// <summary>
        /// Middleware function that prompts for human confirmation before executing sensitive operations.
        /// </summary>
        public static async ValueTask<object?> ConfirmSensitiveOperationAsync(
            AIAgent callingAgent, 
            FunctionInvocationContext context, 
            Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, 
            CancellationToken cancellationToken)
        {
            // Check if this is a sensitive function that requires confirmation
            if (SensitiveFunctions.Contains(context.Function.Name))
            {
                WriteOut.MsgYellow($"\n⚠️  HUMAN CONFIRMATION REQUIRED ⚠️");
                WriteOut.MsgYellow($"The agent wants to execute: {context.Function.Name}");
                
                if (context.Arguments != null && context.Arguments.Count > 0)
                {
                    WriteOut.MsgYellow("With the following parameters:");
                    foreach (var arg in context.Arguments)
                    {
                        WriteOut.MsgYellow($"  - {arg.Key}: {arg.Value}");
                    }
                }

                WriteOut.MsgYellow("\nDo you want to proceed? (yes/no): ");
                var response = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (response != "yes" && response != "y")
                {
                    WriteOut.MsgRed("❌ Operation cancelled by user.\n");
                    return "Operation cancelled by user.";
                }

                WriteOut.MsgGreen("✓ Operation approved by user.\n");
            }

            // Proceed with the function call
            return await next(context, cancellationToken);
        }

        /// <summary>
        /// Adds a custom function name to the list of sensitive functions that require confirmation.
        /// </summary>
        public static void AddSensitiveFunction(string functionName)
        {
            SensitiveFunctions.Add(functionName);
        }

        /// <summary>
        /// Removes a function name from the list of sensitive functions.
        /// </summary>
        public static void RemoveSensitiveFunction(string functionName)
        {
            SensitiveFunctions.Remove(functionName);
        }

        /// <summary>
        /// Gets the list of currently configured sensitive functions.
        /// </summary>
        public static IReadOnlyCollection<string> GetSensitiveFunctions()
        {
            return SensitiveFunctions.ToList().AsReadOnly();
        }
    }
}
