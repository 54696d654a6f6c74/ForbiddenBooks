using ForbiddenBooks.DatabaseLogic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForbiddenBooks.CLI.Commands
{
    public class ResetDbCommand : Base.Command
    {
        private DataController dc;

        public ResetDbCommand(DataController dc)
        {
            this.dc = dc;
        }

        protected override void Help()
        {
            Console.WriteLine("Resets the database comepletely. Used for presentational purposes!");
        }

        public override void Invoke(string[] flags)
        {
            if (flags.Length > 0 && flags[0] == "help")
            {
                Help();
                return;
            }

            if (flags.Length > 0)
            {
                Console.WriteLine("Too many flags for this command");
                return;
            }

            Console.Write("Are you sure (y/n)? ");
            string input = Console.ReadLine().ToLower();
            while ((input != "y" && input != "n") && (input != "yes" && input != "no"))
            {
                Console.Write("Are you sure (y/n)? :");
                input = Console.ReadLine().ToLower();
            }
            if(input == "n" || input=="no")
                return;

            dc.ResetDB();
        }
    }
}
