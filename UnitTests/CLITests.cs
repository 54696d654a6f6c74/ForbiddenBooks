using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Context;
using ForbiddenBooks.DatabaseLogic.Tables;
using System.IO;
using System;
using ForbiddenBooks.CLI.Utils;
using ForbiddenBooks.CLI;

namespace ForbiddenBooksUnitTests
{
    public class CLITests
    {
        [TestCase]
        public void GenericUtilsFindPersonEntityNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "FindPersonEntityNoExceptionsDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User() { Id = 1, FirstName = "Hakvam", LastName = "Nasa" });

                var input = new StringReader("stokata\nmalinin\nHakvam\nNasa");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                User userFound = GenericUtil.FindEntity<User>(dc);
                User useOriginal = dc.GetEntryById<User>(1);

                Assert.AreEqual(userFound.Id, useOriginal.Id);
                Assert.AreEqual(userFound.FirstName, useOriginal.FirstName);
                Assert.AreEqual(userFound.LastName, useOriginal.LastName);
                Assert.AreNotEqual(userFound, useOriginal);
            }
        }

        [TestCase]
        public void GenericUtilsAddGenreEntityNoExceptionsNoChanging()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GenericUtilsAddGenreEntityNoExceptionsNoChangingDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                var input = new StringReader("soc\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                Genre g = GenericUtil.CreateObject(new Genre());
                Assert.AreEqual("soc", g.Name);
            }
        }

        [TestCase]
        public void GenericUtilsAddGenreEntityNoExceptionsChanging()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GenericUtilsAddGenreEntityNoExceptionsChangingDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                var input = new StringReader("-1\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                Genre g = GenericUtil.CreateObject(new Genre() { Name = "kurzamon" }, true);
                Assert.AreEqual("kurzamon", g.Name);
            }
        }

        [TestCase]
        public void AddCommandItemNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "AddCommandItemNoExceptionsDB")
                           .Options;

            IQueryable<Genre> data = new List<Genre>()
            {
                new Genre { Name = "malki detsa"},
                new Genre { Name = "golemi detsa"},
            }.AsQueryable();

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                string inputString = "";
                foreach (Genre g in data.ToList()) inputString += $"add genre\n{g.Name}\n";
                inputString += "exit\n";

                var input = new StringReader(inputString);
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                List<Genre> l = dc.GetEntries<Genre>();
                Assert.AreEqual(data.ToList()[0].Name, l[0].Name);
                Assert.AreEqual(data.ToList()[1].Name, l[1].Name);
            }
        }

        [TestCase]
        public void AddCommandItemEntityAlreadyContained()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "AddCommandItemEntityAlreadyContainedDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                var input = new StringReader("add genre\nmon\nadd genre\nmon\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual("$: NewName: $: NewName: mon is already present in the database\r\n$: ", output.ToString());
            }
        }

        [TestCase]
        public void AddCommandUserNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "AddCommandUserNoExceptionsDB")
                           .Options;

            IQueryable<User> data = new List<User>()
            {
                new User { Id = 1, AcessLevel = 69, Balance = 420, FirstName = "stokata", LastName = "malinin"},
                new User { Id = 2, AcessLevel = 420, Balance = 69, FirstName = "enizkata", LastName = "hasan"},
            }.AsQueryable();

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                string inputString = "";
                foreach (User u in data.ToList()) inputString += $"add user\n{u.AcessLevel}\n{u.Balance}\n{u.FirstName}\n{u.LastName}\n";
                inputString += "exit\n";

                var input = new StringReader(inputString);
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                List<User> l = dc.GetEntries<User>();

                for(int i = 0;i<l.Count;i++)
                {
                    Assert.AreEqual(data.ToList()[i].FirstName, l[i].FirstName);
                    Assert.AreEqual(data.ToList()[i].LastName, l[i].LastName);
                    Assert.AreEqual(data.ToList()[i].AcessLevel, l[i].AcessLevel);
                    Assert.AreEqual(data.ToList()[i].Balance, l[i].Balance);
                }
            }
        }

        [TestCase]
        public void AddCommandAuthorNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "AddCommandAuthorNoExceptionsDB")
                           .Options;

            IQueryable<Author> data = new List<Author>()
            {
                new Author { FirstName = "stokata", LastName = "malinin"},
                new Author { FirstName = "enizkata", LastName = "hasan"},
            }.AsQueryable();

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                string inputString = "";
                foreach (Author a in data.ToList()) inputString += $"add author\n{a.FirstName}\n{a.LastName}\n";
                inputString += "exit\n";

                var input = new StringReader(inputString);
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                List<User> l = dc.GetEntries<User>();

                for (int i = 0; i < l.Count; i++)
                {
                    Assert.AreEqual(data.ToList()[i].FirstName, l[i].FirstName);
                    Assert.AreEqual(data.ToList()[i].LastName, l[i].LastName);
                }
            }
        }
    
        [TestCase] 
        public void RemoveCommandItemNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "RemoveCommandItemNoExceptionsDB")
                           .Options;

            using(ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Genre { Id = 1, Name = "mon" });
                dc.AddEntity(new Genre { Id = 2, Name = "soc" });

                var input = new StringReader("remove genre\nmon\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual(1, dc.GetEntries<Genre>().Count);
                Assert.AreEqual("soc", dc.GetEntries<Genre>()[0].Name);
            }
        }

        [TestCase]
        public void RemoveCommandItemEmptyTable()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "RemoveCommandItemEmptyTableDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                var input = new StringReader("remove genre\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual("$: Impossible command!\r\nThere are no entries in the table\r\n$: ", output.ToString());
            }
        }

        [TestCase]
        public void RemoveCommandPersonNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "RemoveCommandItemNoExceptionsDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Author { Id = 1, FirstName = "stokata", LastName = "malinin" });
                dc.AddEntity(new Author { Id = 2, FirstName = "enizkata", LastName = "hasan" });

                var input = new StringReader("remove author\nstokata\nmalinin\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual(1, dc.GetEntries<Author>().Count);
                Assert.AreEqual("enizkata", dc.GetEntries<Author>()[0].FirstName);
            }
        }

        [TestCase]
        public void RemoveCommandPersonEmptyTable()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "RemoveCommandPersonEmptyTableDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                var input = new StringReader("remove author\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual("$: Impossible command!\r\nThere are no entries in the table\r\n$: ", output.ToString());
            }
        }

        [TestCase]
        public void UpdateGenreNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateGenreNoExceptionsDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Genre { Id = 1, Name = "soc" });

                var input = new StringReader("update genre\nsoc\nmon\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual(1, dc.GetEntries<Genre>().Count);
                Assert.AreEqual("mon", dc.GetEntries<Genre>()[0].Name);
            }
        }

        [TestCase]
        public void UpdateGenreEntityAlreadyContained()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateGenreEntityAlreadyContainedDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Genre { Id = 1, Name = "soc" });
                dc.AddEntity(new Genre { Id = 2, Name = "mon" });

                var input = new StringReader("update genre\nsoc\nmon\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual("$: Name: NewName: mon is already present in the database\r\n$: ", output.ToString());
            }
        }
    
        [TestCase]
        public void UpdateMarketNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateMarketNoExceptionsDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Market { Id = 1, Name = "cheren" });
                dc.AddEntity(new Market { Id = 2, Name = "v dimitrovgrad" });

                var input = new StringReader("update market\ncheren\nbyal\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual(2, dc.GetEntries<Market>().Count);
                Assert.AreEqual("byal", dc.GetEntries<Market>()[0].Name);
            }
        }

        [TestCase]
        public void UpdateMarketEntityAlreadyContained()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateMarketEntityAlreadyContainedDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Market { Id = 1, Name = "cheren" });
                dc.AddEntity(new Market { Id = 2, Name = "byal" });

                var input = new StringReader("update market\ncheren\nbyal\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual("$: Name: NewName: byal is already present in the database\r\n$: ", output.ToString());
            }
        }

        [TestCase]
        public void UpdateAuthorNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateAuthorNoExceptionsDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Author { Id = 1, FirstName = "Maxim", LastName = "Maximov" });
                dc.AddEntity(new Author { Id = 2, FirstName = "Pencho", LastName = "Slaveikov" });

                var input = new StringReader("update author\nPencho\nSlaveikov\nPetko\n-1\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual(2, dc.GetEntries<Author>().Count);
                Assert.AreEqual("Petko", dc.GetEntries<Author>()[1].FirstName);
                Assert.AreEqual("Slaveikov", dc.GetEntries<Author>()[1].LastName);
            }
        }

        [TestCase]
        public void UpdateAuthorEntityAlreadyContained()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateAuthorEntityAlreadyContainedDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Author { Id = 1, FirstName = "Maxim", LastName = "Maximov" });
                dc.AddEntity(new Author { Id = 2, FirstName = "Ivan", LastName = "Vazov" });

                var input = new StringReader("update author\nMaxim\nMaximov\nIvan\nVazov\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual("$: FirstName: LastName: NewFirstName: NewLastName: Ivan Vazov is already present in the database\r\n$: ", output.ToString());
            }
        }

        [TestCase]
        public void UpdateUserNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateUserNoExceptionsDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User { Id = 1, FirstName = "Nadya", LastName = "Koleva", AcessLevel = 69, Balance = 420 });
                dc.AddEntity(new User { Id = 2, FirstName = "Yoanko", LastName = "Mihailova" });
                dc.AddEntity(new User { Id = 3, FirstName = "Stokata", LastName = "Malinin" });

                var input = new StringReader("update user\nNadya\nKoleva\n420\n-1\n-1\nVulkova\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual(3, dc.GetEntries<User>().Count);
                Assert.AreEqual("Nadya", dc.GetEntries<User>()[0].FirstName);
                Assert.AreEqual("Vulkova", dc.GetEntries<User>()[0].LastName);
                Assert.AreEqual(420, dc.GetEntries<User>()[0].AcessLevel);
                Assert.AreEqual(420, dc.GetEntries<User>()[0].Balance);
            }
        }

        [TestCase]
        public void UpdateUserEntityAlreadyContained()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateUserEntityAlreadyContainedDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User { Id = 1, FirstName = "Maxim", LastName = "Maximov" });
                dc.AddEntity(new User { Id = 2, FirstName = "Vanya", LastName = "Vlashinska" });

                var input = new StringReader("update user\nMaxim\nMaximov\n-1\n-1\nVanya\nVlashinska\nexit\n");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                UserInputHandler uih = new UserInputHandler(dc);
                uih.ReadInput();

                Assert.AreEqual("$: FirstName: LastName: NewAcessLevel: NewBalance: NewFirstName: NewLastName: Vanya Vlashinska is already present in the database\r\n$: ", output.ToString());
            }
        }
    }
}
