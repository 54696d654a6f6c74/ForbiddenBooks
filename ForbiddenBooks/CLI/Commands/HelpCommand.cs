using System;
using System.Collections.Generic;

namespace ForbiddenBooks.CLI.Commands
{
    public class HelpCommand : Base.Command
    {
        private Dictionary<string, Base.Command> allCmds;

        protected override void Help()
        {
            Console.WriteLine("Are you for real?");
            Console.WriteLine();
        }

        public HelpCommand(Dictionary<string, Base.Command> allCommands)
        {
            allCmds = allCommands;
        }

        public override void Invoke(string[] flags)
        {
            if(flags.Length == 0)
            {
                foreach(KeyValuePair<string, Base.Command> cmd in allCmds)
                {
                    if (cmd.Key != "help")
                    {
                        Console.WriteLine("Command: " + cmd.Key);
                        Console.WriteLine();
                        cmd.Value.Invoke(new string[] { "help" });
                        Console.WriteLine("\n");
                    }
                }
            }

            else if(flags.Length > 0 && flags.Length < 2)
            {
                if (flags[0] == "help")
                {
                    Console.WriteLine("Nice try :)");
                    return;
                }
                allCmds[flags[0]].Invoke(new string[] { "help" });
            }
        }
    }
}
