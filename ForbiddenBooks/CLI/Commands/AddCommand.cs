using System;
using System.Linq;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.CLI.Utils;

namespace ForbiddenBooks.CLI.Commands
{
    public class AddCommand : Base.Command
    {
        private DataController dc;

        public AddCommand(DataController dc)
        {
            this.dc = dc;
        }

        protected override void Help()
        {
            string[] allTypes = { "user", "market", "magazine", "genre", "author" };

            Console.WriteLine("Function: Creates an entry of a specifed type into the database.\n");
            Console.WriteLine("Flags: ");
            foreach(string type in allTypes)
            {
                Console.WriteLine("{0} -> Specifies that a {0} entry is being added", type);
                Console.WriteLine();
            }
        }

        public override void Invoke(string[] flags)
        {
            if(flags[0] == "help")
            {
                Help();
                return;
            }

            if(flags.Length > 1)
            {
                Console.WriteLine("Too many flags for this command");
                return;
            }

            if(flags[0] == "magazine" || flags[0] == "mag")
            {
                Magazine mag = GenericUtil.CreateMag(new Magazine(), dc);
                if(mag!=null) dc.AddEntity(mag);
                return;
            }

            string flag = flags[0];
            flag = flag.First().ToString().ToUpper() + flag.Substring(1);
            flag = "ForbiddenBooks.DatabaseLogic.Tables." + flag;
            Type T = Type.GetType(flag);

            object newObj = Activator.CreateInstance(T);
            newObj = GenericUtil.CallGenericMethodFromClass<GenericUtil>(T, "CreateObject", this, newObj, false);
            GenericUtil.CallGenericMethodFromClass<DataController>(T, "AddEntity", dc, newObj);
        }        
    }
}
