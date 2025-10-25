using MAF.Assistants.Runners;
using MAF.Assistants.Utility;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;

namespace MAF.Assistants
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Hello, Starting application!");
            WriteOut.Divider(true);

            //LocalProcess.Start();

            //await AzureClient.StartChat<SimpleChat>();
            //await LocalClient.Start<SimpleChat>();

            await AzureClient.StartChat<SimpleChatWithTools>();
            //await LocalClient.StartChat<SimpleChatWithTools>();


            // Temp, just get started with processing something.
            //SimpleChat chat = new SimpleChat();
            //await chat.StartAsync();


            //End of program
            Utility.WriteOut.EndOfProgram();
        }
    }
}

