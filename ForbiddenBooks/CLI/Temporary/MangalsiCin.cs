using System;
using System.Collections.Generic;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Tables;

namespace ForbiddenBooks.CLI.Temporary
{
    public class MangalsiCin
    {
        // 1 -> mangalski print
        // 2 -> mangalski add
        // 3 -> sell mag on market
        // 4 -> user buy mag from market
        // 5 -> user sell mag on market
        // 6 -> mangalski remove
        // 7 -> mangalski update

        public readonly DataController dc;
        OutputHandler printer;

        public MangalsiCin()
        {
            dc = new DataController();
            printer = new OutputHandler(dc);
        }

        private void PrintPrompt()
        {
            Console.WriteLine("1 -> User");
            Console.WriteLine("2 -> Author");
            Console.WriteLine("3 -> Genre");
            Console.WriteLine("4 -> Magazine");
            Console.WriteLine("5 -> Market");
            Console.WriteLine("0 -> Exit");
        }

        string command;
        public void ParseInput()
        {
            while(true)
            {
                Console.WriteLine("1 -> Print");
                Console.WriteLine("2 -> Add");
                Console.WriteLine("3 -> SellMagOnMarket");
                Console.WriteLine("4 -> UserBuyMag");
                Console.WriteLine("5 -> UserSellMag");
                Console.WriteLine("6 -> Remove");
                Console.WriteLine("7 -> Update");

                command = Console.ReadLine();

                switch(command)
                {
                    default:
                        Console.WriteLine("This is not a valid command");
                        break;
                    case "1":
                        MangPrint();
                        Console.WriteLine();
                        break;
                    case "2":
                        MangAdd();
                        break;
                    case "3":
                        SellMagOnMarket();
                        break;
                    case "4":
                        UserBuyMag();
                        break;
                    case "5":
                        UserSellMag();
                        break;
                    case "6":
                        MangRemove();
                        break;
                    case "7":
                        MangUpdate();
                        break;
                }
            }
        }

        private void MangPrint()
        {
            while(true)
            {
                Console.WriteLine("1 -> All");
                Console.WriteLine("2 -> User");
                Console.WriteLine("3 -> Author");
                Console.WriteLine("4 -> Genre");
                Console.WriteLine("5 -> Magazine");
                Console.WriteLine("6 -> Market");
                Console.WriteLine("0 -> Exit");

                string printCmd = Console.ReadLine();

                switch(printCmd)
                {
                    default:
                        Console.WriteLine("This is not a valid command");
                        break;
                    case "1":
                        printer.PrintDatabase();
                        return;
                    case "2":
                        foreach (User u in dc.GetEntries<User>())
                            printer.PrintUser(u, mags: true);
                        return;
                    case "3":
                        foreach (Author a in dc.GetEntries<Author>())
                            printer.PrintAuthor(a);
                        return;
                    case "4":
                        foreach (Genre g in dc.GetEntries<Genre>())
                            printer.PrintGenre(g);
                        return;
                    case "5":
                        foreach (Magazine m in dc.GetEntries<Magazine>())
                            printer.PrintMagazine(m);
                        return;
                    case "6":
                        foreach (Market m in dc.GetEntries<Market>())
                            printer.PrintMarket(m);
                        return;
                    case "0":
                        return;
                }
            }
        }

        private User CreateUser()
        {
            User u = new User();

            Console.Write("FirstName: "); u.FirstName = Console.ReadLine();
            Console.Write("LastName: "); u.LastName = Console.ReadLine();
            Console.Write("Acess level: "); u.AcessLevel = int.Parse(Console.ReadLine());
            Console.Write("Balance: "); u.Balance = decimal.Parse(Console.ReadLine());

            return u;
        }

        private Author CreateAuthor()
        {
            Author a = new Author();

            Console.Write("FirstName: "); a.FirstName = Console.ReadLine();
            Console.Write("LastName: "); a.LastName = Console.ReadLine();

            return a;
        }

        private Genre CreateGenre()
        {
            Genre g = new Genre();

            Console.Write("GenreName: "); g.Name = Console.ReadLine();

            return g;
        }

        private Genre GetGenre(DbQuery querry)
        {
            string genreName;
            while(true)
            {
                Console.Write("GenreName: "); genreName = Console.ReadLine();

                List<Genre> matches = querry.GetGenresByName(genreName);
                if (matches.Count == 0)
                {
                    Console.WriteLine("No such genre in database");
                    continue;
                }
                else
                    return matches[0];
            }
        }

        private Author GetAuthor(DbQuery querry)
        {
            string firstName, lastName;
            while (true)
            {
                Console.Write("Author First Name: "); firstName = Console.ReadLine();
                Console.Write("Author Last Name: "); lastName = Console.ReadLine();

                List<Author> matches = querry.GetAuthorsByBothNames(firstName, lastName);
                if (matches.Count == 0)
                {
                    Console.WriteLine("No such author in database");
                    continue;
                }
                else
                    return matches[0];
            }
        }

        private Magazine CreateMagazine()
        {
            DbQuery querry = new DbQuery(dc);

            if(dc.GetEntries<Author>().Count==0 || dc.GetEntries<Genre>().Count==0)
            {
                Console.WriteLine("Impossible, there are missing entries");
                return null;
            }

            string name;
            Magazine m = new Magazine();

            Console.Write("MagazineName: "); m.Name = Console.ReadLine();
            Console.Write("MagazinePrice: "); m.Price = int.Parse(Console.ReadLine());
            Console.Write("AccessLevel: "); m.AccsessLevel = int.Parse(Console.ReadLine());

            m.Genre = GetGenre(querry);
            m.Author = GetAuthor(querry);

            return m;
        }

        private Market CreateMarket()
        {
            Market m = new Market();

            Console.Write("MarketName: "); m.Name = Console.ReadLine();

            return m;
        }

        private void MangAdd()
        {
            while(true)
            {
                PrintPrompt();

                string addCmd = Console.ReadLine();

                switch(addCmd)
                {
                    default:
                        Console.WriteLine("Not a valid command");
                        break;
                    case "1":
                        User newUser = CreateUser();
                        dc.AddEntity(newUser);
                        return;
                    case "2":
                        Author newAuthor = CreateAuthor();
                        dc.AddEntity(newAuthor);
                        return;
                    case "3":
                        Genre newGenre = CreateGenre();
                        dc.AddEntity(newGenre);
                        return;
                    case "4":
                        Magazine newMag = CreateMagazine();
                        if(newMag!=null) dc.AddEntity(newMag);
                        return;
                    case "5":
                        Market newMarket = CreateMarket();
                        dc.AddEntity(newMarket);
                        return;
                    case "0":
                        return;
                }
            }
        }

        private void SellMagOnMarket()
        {
            DbQuery querry = new DbQuery(dc);
            string mag;
            string market;

            Magazine magEntry;
            Market marketEntry;

            magEntry = GetMagazine(querry);
            marketEntry = GetMarket(querry);

            querry.SellMagazineOnMarket(magEntry, marketEntry);
        }

        private void UserBuyMag()
        {
            DbQuery querry = new DbQuery(dc);

            string userFirst, userLast;
            string mag;
            string market;

            User userEntry;
            Magazine magEntry;
            Market marketEntry;

            // TO DO: Print prompt

            userEntry = GetUser(querry);
           
            Console.WriteLine("All accessible magazines:");
            List<Magazine> accessibleMagazines = querry.GetAccessibleMagazines(userEntry);
            foreach (Magazine m in accessibleMagazines)
            {
                printer.PrintMagazine(m);
            }
            Console.WriteLine();

            magEntry = GetMagazine(querry);
            marketEntry = GetMarket(querry);

            querry.UserBuyMagazine(userEntry, magEntry, marketEntry);
        }

        private void UserSellMag()
        {
            // TO DO: Print prompt

            DbQuery querry = new DbQuery(dc);

            string userFirst, userLast;
            string mag;
            string market;

            User userEntry;
            Magazine magEntry;
            Market marketEntry;

            userEntry = GetUser(querry);
            magEntry = GetMagazine(querry);
            marketEntry = GetMarket(querry);

            querry.UserSellMagazine(userEntry, magEntry, marketEntry);
        }

        private Magazine GetMagazine(DbQuery querry)
        {
            while(true)
            {
                string magName;

                Console.Write("MagazineName: "); magName = Console.ReadLine();
                List<Magazine> magMatches = querry.GetMagazinesByName(magName);

                if (magMatches.Count == 0)
                {
                    Console.WriteLine("No such magazine");
                    continue;
                }

                return magMatches[0];
            }
        }


        private Market GetMarket(DbQuery querry)
        {
            while(true)
            {
                string marketName;

                Console.Write("MarketName: "); marketName = Console.ReadLine();

                List<Market> marketMatches = querry.GetMarketsByName(marketName);
                if (marketMatches.Count == 0)
                {
                    Console.WriteLine("No such market");
                    continue;
                }

                return marketMatches[0];
            }
        }

        private void MangRemove()
        {
            DbQuery querry = new DbQuery(dc);

            while(true)
            {
                PrintPrompt();

                string remCmd = Console.ReadLine();
                switch(remCmd)
                {
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                    case "1":
                        dc.RemoveEntity(GetUser(querry));
                        return;
                    case "2":
                        dc.RemoveEntity(GetAuthor(querry));
                        return;
                    case "3":
                        dc.RemoveEntity(GetGenre(querry));
                        return;
                    case "4":
                        dc.RemoveEntity(GetMagazine(querry));
                        return;
                    case "5":
                        dc.RemoveEntity(GetMarket(querry));
                        return;
                    case "0":
                        return;
                }       
            }
        }

        private User GetUser(DbQuery querry)
        {
            User u = new User { };
            while(true)
            {
                string userFirstName;
                string userLastName;

                Console.Write("UserFirstName: "); userFirstName = Console.ReadLine();
                Console.Write("UserLastName: "); userLastName = Console.ReadLine();

                List<User> userMatches = querry.GetUsersByBothNames(userFirstName, userLastName);
                if (userMatches.Count == 0)
                {
                    Console.WriteLine("No such user");
                    continue;
                }

                
                return u;
            }
        }

        private void UpdateUser(DbQuery querry)
        {
            User u = GetUser(querry);
            string help;

            Console.Write("NewUserFirstName: ");help = Console.ReadLine();
            if (help != "-1") u.FirstName = help;

            Console.Write("NewUserLastName: "); help = Console.ReadLine();
            if (help != "-1") u.LastName = help;

            Console.Write("NewUserBalance: ");help = Console.ReadLine();
            if (help != "-1") u.Balance = decimal.Parse(help);

            Console.Write("NewUserAccessLevel: ");help = Console.ReadLine();
            if (help != "-1") u.AcessLevel = int.Parse(help);

            dc.UpdateEntry(u);
        }

        private void UpdateMagazine(DbQuery querry)
        {
            Magazine m = GetMagazine(querry);
            string help;

            Console.Write("NewMagazineName: ");help = Console.ReadLine();
            if (help != "-1") m.Name = help;

            Console.Write("NewMagazineAccessLevel: "); help = Console.ReadLine();
            if (help != "-1") m.AccsessLevel = int.Parse(help);

            Console.Write("NewMagazineGenre: "); help = Console.ReadLine();
            if(help!="-1")
            {
                while (true)
                {
                    List<Genre> genreMatches = querry.GetGenresByName(help);
                    if(genreMatches.Count==0)
                    {
                        Console.WriteLine("No such genre");
                        Console.Write("NewMagazineGenre: "); help = Console.ReadLine();

                        continue;
                    }

                    m.Genre = genreMatches[0];
                    break;
                }
            }

            string help1;
            Console.Write("NewMagazineAuthorFirstName: "); help = Console.ReadLine();
            Console.Write("NewMagazineAuthorLastName: "); help1 = Console.ReadLine();
            if (help != "-1" && help1!="-1")
            {
                while (true)
                {
                    List<Author> authorMatches = querry.GetAuthorsByBothNames(help, help1);
                    if (authorMatches.Count == 0)
                    {
                        Console.WriteLine("No such author");
                        Console.Write("NewMagazineAuthorFirstName: "); help = Console.ReadLine();
                        Console.Write("NewMagazineAuthorLastName: "); help1 = Console.ReadLine();

                        continue;
                    }

                    m.Author = authorMatches[0];
                    break;
                }
            }

            dc.UpdateEntry(m);
        }

        private void UpdateAuthor(DbQuery querry)
        {
            Author a = GetAuthor(querry);

            string help;

            Console.Write("NewAuthorFirstName: "); help = Console.ReadLine();
            if  (help != "-1") a.FirstName = help;

            Console.Write("NewAuthorLastName: "); help = Console.ReadLine();
            if (help != "-1") a.LastName = help;

            dc.UpdateEntry(a);
        }
        
        private void UpdateGenre(DbQuery querry)
        {
            Genre g = GetGenre(querry);

            string help;

            Console.Write("NewGenreName: "); help = Console.ReadLine();
            if  (help != "-1") g.Name = help;

            dc.UpdateEntry(g);            
        }

        private void UpdateMarket(DbQuery querry)
        {
            Market m = GetMarket(querry);

            string help;

            Console.Write("NewMarketName: "); help = Console.ReadLine();
            if  (help != "-1") m.Name = help;

            dc.UpdateEntry(m);
        }

        private void MangUpdate()
        {
            DbQuery querry = new DbQuery(dc);
            while(true)
            {
                PrintPrompt();

                string updateCmd = Console.ReadLine();
                switch(updateCmd)
                {
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                    case "1":
                        UpdateUser(querry);
                        return;
                    case "2":
                        UpdateAuthor(querry);
                        return;
                    case "3":
                        UpdateGenre(querry);
                        return;
                    case "4":
                        UpdateMagazine(querry);
                        return;
                    case "5":
                        UpdateMarket(querry);
                        return;
                    case "0":
                        return;
                }
            }
        }
    }
}
