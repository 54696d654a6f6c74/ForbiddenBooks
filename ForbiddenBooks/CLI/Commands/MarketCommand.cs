using System;
using System.Linq;
using System.Collections.Generic;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Tables.Base;

namespace ForbiddenBooks.CLI.Commands
{
    public class MarketCommand : Base.Command
    {
        private DataController dc;
        public MarketCommand(DataController dc)
        {
            this.dc = dc;
        }

        protected override void Help()
        {
            Console.WriteLine("Function: Executes an action for a specifed market.\n");
            Console.WriteLine("Flags: ");
            Console.WriteLine("sell -> Adds a specifed magazine to the specifed market");
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
            if(flags[0]!="sell")
            {
                Console.WriteLine("Invalid command");
                return;
            }

            OutputHandler printer = new OutputHandler(dc);
            DbQuery query = new DbQuery(dc);

            if (query.IsTableEmpty<Magazine>() == true)
            {
                Console.WriteLine("Impossible command: there are no magazines in the database");
                return;
            }
            if (query.IsTableEmpty<Market>() == true)
            {
                Console.WriteLine("Impossible command: there are no markets in the database");
                return;
            }

            List<Magazine> available = query.GetNotOwnedMagazines();
            if (available.Count == 0)
            {
                Console.WriteLine("Impossible command: there are no available magazines");
                return;
            }

            Console.WriteLine("Available Magazines: ");
            foreach (Magazine m in available)
            {
                printer.PrintMagazine(m);
            }
            Console.WriteLine();

            Console.WriteLine("Market info...");
            Market market = GetEntity<Market>();
            Console.WriteLine();

            Console.WriteLine("Magazine info...");
            Magazine mag = GetEntity<Magazine>();
            Console.WriteLine();

            query.SellMagazineOnMarket(mag, market);
        }

        public T GetEntity<T>() where T : class
        {
            DbQuery query = new DbQuery(dc);

            object[] paramS = { };
            string methodName = "";
            string actualType = (typeof(T).ToString()).Split('.').Last();

            while (true)
            {
                if (typeof(T).BaseType == typeof(Item))
                {
                    string name;
                    Console.Write("Name: "); name = Console.ReadLine();

                    paramS = new object[] { name };
                    methodName = $"Get{actualType}sByName";
                }
                else if (typeof(T).BaseType == typeof(Person))
                {
                    string FirstName, LastName;

                    Console.Write("FirstName: "); FirstName = Console.ReadLine();
                    Console.Write("LastName: "); LastName = Console.ReadLine();

                    paramS = new object[] { FirstName, LastName };
                    methodName = $"Get{actualType}sByBothNames";
                }

                object matches = typeof(DbQuery).GetMethod(methodName).Invoke(query, paramS);
                if ((matches as List<T>).Count == 0)
                {
                    Console.WriteLine($"No such {actualType}!");
                    continue;
                }

                return (matches as List<T>)[0];
            }
        }
    }
}
