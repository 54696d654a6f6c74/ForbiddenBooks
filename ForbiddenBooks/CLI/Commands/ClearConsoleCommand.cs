using System;

namespace ForbiddenBooks.CLI.Commands
{
    public class ClearConsoleCommand : Base.Command
    {
        protected override void Help()
        {
            Console.WriteLine("Clears the console. It's that simple...");
        }

        public override void Invoke(string[] flags)
        {
            if (flags.Length > 0)
            {
                if (flags[0] == "help")
                {
                    Help();
                    return;
                }
            }
            Console.Clear();
        }
    }
}
