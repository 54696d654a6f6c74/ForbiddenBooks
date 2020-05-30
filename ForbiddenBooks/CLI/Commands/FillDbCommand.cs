using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForbiddenBooks.CLI.Commands
{
    public class FillDbCommand : Base.Command
    {
        private DataController dc;

        public FillDbCommand(DataController dc)
        {
            this.dc = dc;
        }

        protected override void Help()
        {
            Console.WriteLine("Fills the database with dummy entries. Used for presentational purposes!");
        }

        public override void Invoke(string[] flags)
        {
            if(flags.Length > 0 && flags[0] == "help")
            {
                Help();
                return;
            }
            
            if(flags.Length > 0)
            {
                Console.WriteLine("Too many flags for this command");
                return;
            }

            Genre[] genres =
            { 
                new Genre() { Name = "amateur" },
                new Genre() { Name = "feet"},
                new Genre() { Name = "interactive"},
                new Genre() { Name = "reality"}
            };

            foreach (Genre g in genres) dc.AddEntity(g);

            Author[] authors =
            {
                new Author { FirstName = "Peter", LastName = "Johnson" },
                new Author { FirstName = "Amir", LastName = "Khan" },
                new Author { FirstName = "Nick", LastName = "Johnson" },
                new Author { FirstName = "Stokata", LastName = "Pulev" }
            };
            
            foreach (Author a in authors) dc.AddEntity(a);

            User[] users =
            {
                new User { FirstName = "Stoyan", LastName = "Malinin", AcessLevel = 69, Balance = 100 },
                new User { FirstName = "Eniz", LastName = "Hasan", AcessLevel = 42, Balance = 20 },
                new User { FirstName = "Manol", LastName = "Monov", AcessLevel = 10, Balance = 10000 }
            };

            foreach (User u in users) dc.AddEntity(u);

            Market[] markets =
            {
                new Market { Name = "Black" },
                new Market { Name = "White" },
                new Market { Name = "Dimitrovgrad" }
            };

            foreach(Market m in markets) dc.AddEntity(m);

            Magazine[] magazines =
            {
                new Magazine {Name = "Cosmos", Genre = genres[3], AccsessLevel = 5, Author = authors[3], Price = 25, marketOwner = markets[2]},
                new Magazine {Name = "Cosmopolitan", Genre = genres[2], AccsessLevel = 20, Author = authors[3], Price = 100, userOwner = users[1]},
                new Magazine {Name = "Lonely nights", Genre = genres[0], AccsessLevel = 69, Author = authors[1], Price = 15},
                new Magazine {Name = "Party in the USA", Genre = genres[1], AccsessLevel = 40, Author = authors[2], Price = 60}
            };

            foreach (Magazine m in magazines) dc.AddEntity(m);
        }
    }
}
