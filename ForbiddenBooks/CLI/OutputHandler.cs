using System;
using System.Collections.Generic;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Tables;

namespace ForbiddenBooks.CLI
{
    public class OutputHandler
    {
        private readonly DataController dc;
        public OutputHandler(DataController dc)
        {
            this.dc = dc;
        }

        private void AddIndents(int indents)
        {
            for (int i = 0; i < indents; i++)
                Console.Write("\t");
        }

        public void PrintUser(User u, int indentCount = 0, bool mags = false)
        {
            AddIndents(indentCount);
            Console.WriteLine("{0} --> {1} {2}", u.Id, u.FirstName, u.LastName);
            AddIndents(indentCount + 1);
            Console.WriteLine("Access level: {0}", u.AcessLevel);
            AddIndents(indentCount + 1);
            Console.WriteLine("Balance: {0}", u.Balance);
            Console.WriteLine("\tOwns:");
            if (mags)
            {
                foreach (Magazine m in u.Magazines)
                    PrintMagazine(m, indentCount + 1);
            }
        }

        public void PrintMagazine(Magazine m, int indentCount = 0)
        {
            AddIndents(indentCount);
            Console.WriteLine("{0} --> {1}", m.Id, m.Name);
            AddIndents(indentCount + 1);
            Console.WriteLine("By: {0} {1} Genre: {2}", m.Author.FirstName, m.Author.LastName, m.Genre.Name);
            AddIndents(indentCount + 1);
            Console.WriteLine("Costs: {0}$ Requires: {1} access level", m.Price, m.AccsessLevel);
        }

        public void PrintMarket(Market m, int indentCount = 0)
        {
            AddIndents(indentCount);
            Console.WriteLine("{0} --> {1}", m.Id, m.Name);
            AddIndents(indentCount);
            Console.WriteLine("Magazines on sale:");
            foreach (Magazine mag in m.Magazines)
                PrintMagazine(mag, indentCount + 1);
        }

        public void PrintAuthor(Author a, int indentCount = 0)
        {
            AddIndents(indentCount);
            Console.WriteLine("{0} --> {1} {2}", a.Id, a.FirstName, a.LastName);
        }

        public void PrintGenre(Genre g, int indentCount = 0)
        {
            AddIndents(indentCount);
            Console.WriteLine("{0} --> {1}", g.Id, g.Name);
        }

        public void PrintDatabase()
        {
            List<User> users = dc.GetEntries<User>();
            List<Market> markets = dc.GetEntries<Market>();
            List<Magazine> mags = dc.GetEntries<Magazine>();
            List<Author> authors = dc.GetEntries<Author>();
            List<Genre> genres = dc.GetEntries<Genre>();

            Console.WriteLine("---------------------------------");
            Console.WriteLine("All entries in the DB:");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("\nUsers table:");

            foreach(User u in users)
            {
                PrintUser(u, 1, true);
            }

            Console.WriteLine("*********************************");
            Console.WriteLine("\nMarkerts table:");

            foreach(Market m in markets)
            {
                PrintMarket(m, 1);
            }

            Console.WriteLine("*********************************");
            Console.WriteLine("\nMagazines table:");

            foreach(Magazine m in mags)
                PrintMagazine(m, 1);

            Console.WriteLine("*********************************");
            Console.WriteLine("\nAuthors table:");

            foreach (Author a in authors)
                PrintAuthor(a, 1);

            Console.WriteLine("*********************************");
            Console.WriteLine("\nGenres table:");

            foreach (Genre g in genres)
                PrintGenre(g, 1);
        }
    }
}
