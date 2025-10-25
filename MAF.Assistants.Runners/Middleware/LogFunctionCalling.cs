using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MAF.Assistants.Middleware
{
    internal class LogFunctionCalling
    {
        public static async ValueTask<object?> LogFunctionCallAsync(AIAgent callingAgent, FunctionInvocationContext context, Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken cancellationToken)
        {
            
            StringBuilder logBuilder = new StringBuilder();
            logBuilder.AppendLine($" - Function Call Invoked: '{context.Function.Name}'");

            if(context.Arguments != null && context.Arguments.Count > 0)
            {
                logBuilder.AppendLine("   - Arguments:");
                foreach (var arg in context.Arguments)
                {
                    logBuilder.AppendLine($"     - {arg.Key}: {arg.Value}");
                }
            }
            else
            {
                logBuilder.AppendLine("   - No arguments provided.");
            }

            WriteOut.MsgGrey(logBuilder.ToString());

            return await next(context, cancellationToken);
        }

    }
}
