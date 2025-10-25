using Azure.AI.OpenAI;
using MAF.Assistants.Abstract;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Middleware;
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
    internal class SimpleChatWithTools : BaseChat, RunChatModel
    {
        
        public SimpleChatWithTools() {
            RequiresTools = true;
        }
        
        public async Task StartAsync(ChatClient client)
        {
            //Note this doesnt work with local foundry yet, this is a limitation of the model used
            

            WriteOut.MsgCyan("Starting Simple Chat with Tools...");
            WriteOut.MsgBlankLine();

            string modelInstruction = "You are a helpful assistant";
            string agentName = "WeatherAI";
            string prompt = "What is the weather like in Amsterdam?";

            AIAgent agent = client.CreateAIAgent(instructions: modelInstruction, name: agentName, 
                tools: [AIFunctionFactory.Create(GetWeather, "get_weather", "Gets the current weather for a specified location")])
                .AsBuilder()
                .Use(LogFunctionCalling.LogFunctionCallAsync)
                .Build();

            // Simple Example - Streaming
            // TODO: Move away from console.writeline and use the WriteOut utility class
            //Console.WriteLine(await agent.RunAsync(prompt));

            await foreach (var update in agent.RunStreamingAsync(prompt))
            {
                Console.Write(update);
            }

        }


        //Test tool to get weather information
        [Description("Get the weather for a given location.")]
        static string GetWeather([Description("The location to get the weather for.")] string location)
            => $"The weather in {location} is cloudy with a high of 15°C.";

    }
}
