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
            Console.WriteLine("Hello, Please Select the Help you need!");
            WriteOut.Divider(true);

            //LocalProcess.Start();
            
            //await AzureClient.Start<SimpleChat>();
            await LocalClient.Start<SimpleChat>();

            // Temp, just get started with processing something.
            //SimpleChat chat = new SimpleChat();
            //await chat.StartAsync();


            //End of program
            Utility.WriteOut.EndOfProgram();
        }
    }
}

