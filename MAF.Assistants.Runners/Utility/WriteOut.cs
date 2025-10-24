using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Utility
{
    internal class WriteOut
    {

        public static void Msg(string input)
        {
            Console.WriteLine(input);
        }

        public static void MsgBlankLine()
        {
            Console.WriteLine();
        }

        public static void MsgGrey(string input)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        public static void MsgCyan(string input)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(input);
            Console.ResetColor();
        }


        public static void MsgGreen(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        public static void EndOfProgram()
        {
            Divider();
            MsgGreen("Press any key to exit...");
            Console.ReadKey();

        }

        public static void Divider()
        {
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
