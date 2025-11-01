using Azure.AI.OpenAI;
using MAF.Assistants.Abstract;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Middleware;
using MAF.Assistants.Tools;
using MAF.Assistants.Utility;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Runners
{
    /// <summary>
    /// Chat runner that demonstrates Microsoft Graph integration with AI agents.
    /// Includes tools for SharePoint and Email operations with human-in-the-loop confirmation.
    /// </summary>
    internal class GraphChatWithTools : BaseChat, RunChatModel
    {
        public GraphChatWithTools()
        {
            RequiresTools = true;
        }

        public async Task StartAsync(ChatClient client)
        {
            WriteOut.MsgCyan("Starting Graph Chat with Microsoft 365 Tools...");
            WriteOut.MsgBlankLine();

            string modelInstruction = @"You are a helpful assistant with access to Microsoft 365 data through Microsoft Graph.
You can:
- List and access SharePoint sites and files
- Read and send emails
- Help users organize and retrieve information from their Microsoft 365 environment

When performing sensitive operations like sending emails or writing files, you must get user confirmation first.
Always be clear about what data you're accessing and what actions you're taking.";

            string agentName = "Microsoft365Assistant";
            
            // Create the AI agent with Microsoft Graph tools
            AIAgent agent = client.CreateAIAgent(
                instructions: modelInstruction, 
                name: agentName,
                tools: [
                    AIFunctionFactory.Create(GraphTools.ListSharePointSites, "list_sharepoint_sites", "Lists available SharePoint sites"),
                    AIFunctionFactory.Create(GraphTools.GetSharePointFiles, "get_sharepoint_files", "Gets files from a SharePoint document library"),
                    AIFunctionFactory.Create(GraphTools.ReadSharePointFile, "read_sharepoint_file", "Reads the content of a file from SharePoint"),
                    AIFunctionFactory.Create(GraphTools.WriteSharePointFile, "write_sharepoint_file", "Writes or updates a file in SharePoint"),
                    AIFunctionFactory.Create(GraphTools.GetRecentEmails, "get_recent_emails", "Gets recent emails from a user's inbox"),
                    AIFunctionFactory.Create(GraphTools.ReadEmail, "read_email", "Reads the full content of a specific email"),
                    AIFunctionFactory.Create(GraphTools.SendEmail, "send_email", "Sends an email to one or more recipients")
                ])
                .AsBuilder()
                .Use(LogFunctionCalling.LogFunctionCallAsync)
                .Use(HumanInTheLoop.ConfirmSensitiveOperationAsync)
                .Build();

            WriteOut.MsgGreen("Agent initialized with Microsoft Graph tools.");
            WriteOut.MsgGrey("Available tools: SharePoint (list sites, read/write files), Email (read, send)");
            WriteOut.Divider();

            // Interactive chat loop
            WriteOut.MsgCyan("Enter your requests (or type 'exit' to quit):");
            WriteOut.MsgBlankLine();

            while (true)
            {
                Console.Write("You: ");
                string? userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                    continue;

                if (userInput.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    WriteOut.MsgGreen("Goodbye!");
                    break;
                }

                Console.Write("Assistant: ");
                await foreach (var update in agent.RunStreamingAsync(userInput))
                {
                    Console.Write(update);
                }
                Console.WriteLine();
                WriteOut.MsgBlankLine();
            }
        }
    }
}
