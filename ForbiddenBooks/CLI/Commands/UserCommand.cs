using System;
using System.Linq;
using System.Collections.Generic;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Tables.Base;

namespace ForbiddenBooks.CLI.Commands
{
    public class UserCommand : Base.Command
    {
        private DataController dc;
        public UserCommand(DataController dc)
        {
            this.dc = dc;
        }

        protected override void Help()
        {
            Console.WriteLine("Function: Executes an action for a specified user.\n");
            Console.WriteLine("Flags: ");
            Console.WriteLine("sell -> Refunds the cost of a specified magazine from a specified user and puts it for sale on specified market.");
            Console.WriteLine();
            Console.WriteLine("buy -> Moves a specified magazine from a specifed market to a specifed user and subtracts the cost of the magazine from the user's balance.");
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

            OutputHandler printer = new OutputHandler(dc);
            DbQuery query = new DbQuery(dc);

            User user;
            Magazine mag;
            Market market;
            List<Magazine> available;

            if(query.IsTableEmpty<User>()==true)
            {
                Console.WriteLine("Impossible command: there are no users in the database");
                return;
            }
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

            Console.WriteLine("User info...");
            user = GetEntity<User>();
            Console.WriteLine();

            if (flags[0] == "sell") available = user.Magazines.ToList();
            else available = query.GetAccessibleMagazines(user);

            if (available.Count == 0)
            {
                Console.WriteLine("Impossible command: there are no available magazines");
                return;
            }

            Console.WriteLine("Available Magazines: ");
            foreach(Magazine m in available)
            {
                printer.PrintMagazine(m);
            }
            Console.WriteLine();

            Console.WriteLine("Magazine info...");
            mag = GetEntity<Magazine>();
            Console.WriteLine();

            Console.WriteLine("Market info...");
            market = GetEntity<Market>();
            Console.WriteLine();

            if (flags[0] == "sell") query.UserSellMagazine(user, mag, market);
            else query.UserBuyMagazine(user, mag, market);
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
