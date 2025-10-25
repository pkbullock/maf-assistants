using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Utility
{
    internal class WriteOut
    {

        public static void Msg(string input, ConsoleColor? color = null)
        {
            if(color.HasValue)
            {
                Console.ForegroundColor = color.Value;
                Console.WriteLine(input);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(input);
            }    
        }

        public static void MsgBlankLine()
        {
            Console.WriteLine();
        }

        public static void MsgGrey(string input)
        {
            Msg(input, ConsoleColor.DarkGray);
        }

        public static void MsgCyan(string input)
        {
            Msg(input, ConsoleColor.Cyan);
        }


        public static void MsgGreen(string input)
        {
            Msg(input, ConsoleColor.Green);
        }

        public static void MsgYellow(string input)
        {
            Msg(input, ConsoleColor.Yellow);
        }

        public static void EndOfProgram()
        {
            Divider();
            MsgGreen("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Creates a divider line across the console window
        /// </summary>
        public static void Divider(bool noNewLine = false)
        {
            if (!noNewLine)
            {
                Console.WriteLine();
            }
            Console.WriteLine("".PadLeft(Console.WindowWidth, '-'));
        }
    }
}
