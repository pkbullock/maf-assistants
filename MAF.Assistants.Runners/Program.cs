using MAF.Assistants.Runners;
using MAF.Assistants.Utility;
using System;
using System.Configuration;
using System.Web;

namespace MAF.Assistants
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Hello, Please Select the Help you need!");

            //LocalProcess.Start();
            //AzureOpenAIProcess.Start();

            var configuration = ConfigurationManager..GetConfiguration();


            //End of program
            Utility.WriteOut.EndOfProgram();
        }
    }
}

