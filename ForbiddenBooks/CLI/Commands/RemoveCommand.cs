using System;
using System.Linq;
using ForbiddenBooks.CLI.Utils;
using ForbiddenBooks.DatabaseLogic;

namespace ForbiddenBooks.CLI.Commands
{
    public class RemoveCommand : Base.Command
    {
        private DataController dc;

        public RemoveCommand(DataController dc)
        {
            this.dc = dc;
        }

        protected override void Help()
        {
            string[] allTypes = { "user", "market", "magazine", "genre", "author" };

            Console.WriteLine("Function: Removes a specifed entry of a specifed type from the database.\n");
            Console.WriteLine("Flags: ");
            foreach (string type in allTypes)
            {
                Console.WriteLine("{0} -> Specifies that an entry of type {0} will be removed.", type);
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Note that removing entries that would break database relations are not allowed!");
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

            string flag = flags[0];
            flag = flag.First().ToString().ToUpper() + flag.Substring(1);
            if (flag == "Mag")
                flag = "Magazine";

            flag = "ForbiddenBooks.DatabaseLogic.Tables." + flag;
            Type T = Type.GetType(flag);

            DbQuery query = new DbQuery(dc);
            bool empty = (bool)GenericUtil.CallGenericMethodFromClass<DbQuery>(T, "IsTableEmpty", query, new object[] { });
            if (!empty)
            {
                object toRemove = GenericUtil.CallGenericMethodFromClass<GenericUtil>(T, "FindEntity", this, dc);
                GenericUtil.CallGenericMethodFromClass<DataController>(T, "RemoveEntity", dc, toRemove);
            }
            else
            {
                Console.WriteLine("Impossible command!");
                Console.WriteLine("There are no entries in the table");
            }
        }
    }
}
