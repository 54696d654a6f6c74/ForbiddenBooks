using System;
using System.Linq;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.CLI.Utils;

namespace ForbiddenBooks.CLI.Commands
{
    public class UpdateCommand : Base.Command
    {
        private DataController dc;

        public UpdateCommand(DataController dc)
        {
            this.dc = dc;
        }

        protected override void Help()
        {
            string[] allTypes = { "user", "market", "magazine", "genre", "author" };

            Console.WriteLine("Function: Updates an entry in the database of a specified type.\n"); ;
            Console.WriteLine("Flags: ");
            foreach (string type in allTypes)
            {
                Console.WriteLine("{0} -> Specifies that an entry of type {0} will be updated", type);
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Note that updating an entry so it would match another one of the same type is not allowed");
        }

        public override void Invoke(string[] flags)
        {
            if (flags[0] == "help")
            {
                Help();
                return;
            }

            if (flags.Length > 1)
            {
                Console.WriteLine("Too many flags for this command");
                return;
            }

            if (flags[0] == "magazine" || flags[0] == "mag")
            {
                Magazine magTarget = GenericUtil.FindEntity<Magazine>(dc);

                Magazine mag = GenericUtil.CreateMag(magTarget, dc, true);
                if (mag != null) dc.UpdateEntry(mag);
                return;
            }

            string flag = flags[0];
            flag = flag.First().ToString().ToUpper() + flag.Substring(1);
            flag = "ForbiddenBooks.DatabaseLogic.Tables." + flag;
            Type T = Type.GetType(flag);

            object target = GenericUtil.CallGenericMethodFromClass<GenericUtil>(T, "FindEntity", this, dc);
            GenericUtil.CallGenericMethodFromClass<GenericUtil>(T, "CreateObject", this, target, true);
            GenericUtil.CallGenericMethodFromClass<DataController>(T, "UpdateEntry", dc, target);
        }

    }
}
