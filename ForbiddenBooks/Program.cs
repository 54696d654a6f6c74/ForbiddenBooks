using System;
using ForbiddenBooks.CLI;

namespace ForbiddenBooks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("For more information type 'help'");

            UserInputHandler uih = new UserInputHandler();
            uih.ReadInput();
        }
    }
}
