using System;
using System.Collections.Generic;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.DatabaseLogic;

namespace ForbiddenBooks.CLI.Commands
{
    // Can be refactored to follow the pattern of the
    // other CRUD commands
    public class PrintCommand : Base.Command
    {
        private delegate void PrintCommand1();
        private delegate void PrintCommand2(string[] flag);
        Dictionary<string, PrintCommand1> flag1;
        Dictionary<string, PrintCommand2> flag2;

        private DataController dc;
        private OutputHandler printer;
        private DbQuery query;

        protected override void Help()
        {
            string[] allTypes = { "users", "markets", "magazines", "genres", "authors" };

            Console.WriteLine("Function: Displays enties of specifed type in the database.\n");
            Console.WriteLine("Flags: ");
            foreach (string type in allTypes)
            {
                Console.WriteLine("{0} -> Specifies that entries of type {0} should be printed.", type);
                Console.WriteLine();
            }
            allTypes = new string[] { "user", "market", "magazine", "genre", "author" };
            Console.WriteLine();
            Console.WriteLine("Additional flags: ");
            foreach(string type in allTypes)
            {
                Console.WriteLine("{0} <name> -> Looks for all entries of type {0} that match <name> in the database.", type);
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Usage: ");
            Console.WriteLine("print all --> prints the entire database (same result canbe achived with only <print>.");
            Console.WriteLine("print magazines --> prints all magazine entries in the database.");
            Console.WriteLine("print magazine <name> --> prints details of the magazine with propert Name that matches <name>");
            Console.WriteLine("print user <name> --> prints all users that either have <name> as their first or last name");
        }

        public PrintCommand(DataController dc, OutputHandler printer)
        {
            this.dc = dc;
            this.printer = printer;
            query = new DbQuery(dc);

            flag1 = new Dictionary<string, PrintCommand1>();
            flag2 = new Dictionary<string, PrintCommand2>();

            flag1.Add("users", PrintAllUsers);
            flag1.Add("authors", PrintAllAuthors);
            flag1.Add("genres", PrintAllGenres);
            flag1.Add("markets", PrintAllMarkets);
            flag1.Add("magazines", PrintAllMagazines);
            flag1.Add("mags", PrintAllMagazines);
            flag1.Add("all", PrintAll);

            flag2.Add("user", PrintUsers);
            flag2.Add("author", PrintAuthors);
            flag2.Add("market", PrintMarkets);
            flag2.Add("magazines", PrintMagazines);
            flag2.Add("mags", PrintMagazines);
            flag2.Add("genres", PrintGenres);
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

            // Possible place for a null referance excpetion
            if (flags.Length == 0)
            {
                PrintAll();
                return;
            }

            if (flags.Length == 1)
            {
                // Find the func that matches the flag
                // and call it
                flag1[flags[0]]();
                return;
            }

            if (flags.Length == 2)
            {
                // Find the func that matches the flag 
                // and call it with the rest of the flags
                flag2[flags[0]](flags[1..flags.Length]);
                return;
            }

            // Remove users and authors funcs because
            // they can't be called with more than 2 flags
            // and redo for > 2 flags
            flag2.Remove("user");
            flag2.Remove("author");
            if (flags.Length > 2)
                flag2[flags[0]](flags[1..flags.Length]);
        }

        void PrintAll()
        {
            printer.PrintDatabase();
        }

        void PrintAllUsers()
        {
            List<User> users = dc.GetEntries<User>();
            foreach (User u in users)
                printer.PrintUser(u, mags:true);
        }

        void PrintAllAuthors()
        {
            List<Author> authors = dc.GetEntries<Author>();
            foreach (Author a in authors)
                printer.PrintAuthor(a);
        }

        void PrintAllMarkets()
        {
            List<Market> markets = dc.GetEntries<Market>();
            foreach (Market m in markets)
                printer.PrintMarket(m);
        }

        void PrintAllGenres()
        {
            List<Genre> genres = dc.GetEntries<Genre>();
            foreach (Genre g in genres)
                printer.PrintGenre(g);
        }

        void PrintAllMagazines()
        {
            List<Magazine> magazines = dc.GetEntries<Magazine>();
            foreach (Magazine m in magazines)
                printer.PrintMagazine(m);
        }

        void PrintUsers(string[] userNames)
        {
            void PrintMathces(List<User> matches, bool firstName)
            {
                if (matches.Count > 0)
                {
                    if (firstName)
                        Console.WriteLine("Users with first name {0}:", userNames[0]);
                    else Console.WriteLine("Users with last name {0}:", userNames[0]);

                    foreach (User u in matches)
                        printer.PrintUser(u, mags:true);
                }
                else
                {
                    if (firstName)
                        Console.WriteLine("No users with first name {0} were found", userNames[0]);
                    else Console.WriteLine("No users with last name {0} were found", userNames[0]);
                }
            }

            if (userNames.Length == 1)
            {
                List<User> matches = query.GetUsersByFirstName(userNames[0]);
                PrintMathces(matches, true);
                matches = query.GetUsersByLastName(userNames[0]);
                PrintMathces(matches, false);
            }
            else if (userNames.Length == 2)
            {
                List<User> matches = query.GetUsersByBothNames(userNames[0], userNames[1]);
                if (matches.Count == 0)
                    Console.WriteLine("No users with those names");
                foreach (User u in matches)
                    printer.PrintUser(u, mags:true);
            }
        }

        void PrintAuthors(string[] authorNames)
        {
            void PrintMathces(List<Author> matches, bool firstName)
            {
                if (matches.Count > 0)
                {
                    if (firstName)
                        Console.WriteLine("Authors with first name {0}:", authorNames[0]);
                    else Console.WriteLine("Authors with last name {0}:", authorNames[0]);

                    foreach (Author a in matches)
                        printer.PrintAuthor(a);
                }
                else
                {
                    if (firstName)
                        Console.WriteLine("No authors with first name {0} were found", authorNames[0]);
                    else Console.WriteLine("No authors with last name {0} were found", authorNames[0]);
                }
            }

            if (authorNames.Length == 1)
            {
                List<Author> matches = query.GetAuthorsByFirstName(authorNames[0]);
                PrintMathces(matches, true);
                matches = query.GetAuthorsByLastName(authorNames[0]);
                PrintMathces(matches, false);
            }
            else if (authorNames.Length == 2)
            {
                List<Author> matches = query.GetAuthorsByBothNames(authorNames[0], authorNames[1]);
                if (matches.Count == 0)
                    Console.WriteLine("No authors with those names");
                foreach (Author a in matches)
                    printer.PrintAuthor(a);
            }
        }

        void PrintMarkets(string[] marketNames)
        {
            foreach (string s in marketNames)
            {
                List<Market> matches = query.GetMarketsByName(s);
                if (matches.Count == 0)
                {
                    Console.WriteLine("Could not find any market called {0}", s);
                    continue;
                }
                foreach (Market m in matches)
                    printer.PrintMarket(m);
            }
        }

        void PrintMagazines(string[] magazineNames)
        {
            foreach (string s in magazineNames)
            {
                List<Magazine> matches = query.GetMagazinesByName(s);
                if (matches.Count == 0)
                {
                    Console.WriteLine("Could not find any magazine called {0}", s);
                    continue;
                }
                foreach (Magazine m in matches)
                    printer.PrintMagazine(m);
            }
        }

        void PrintGenres(string[] genreNames)
        {
            foreach (string s in genreNames)
            {
                List<Genre> matches = query.GetGenresByName(s);
                if (matches.Count == 0)
                {
                    Console.WriteLine("Could not find any genre called {0}", s);
                    continue;
                }
                foreach (Genre g in matches)
                    printer.PrintGenre(g);
            }
        }
    }
}
