using System;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Util
{
    class Logger
    {
        public static Task Message(string message = "", string valueMessage = "")
        {
            // Display message in default
            Console.Write(message);

            if (!string.IsNullOrEmpty(valueMessage))
            {
                // Display value in green
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(valueMessage);
                Console.ResetColor();
            }

            return Task.CompletedTask;
        }

        public static Task ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}
