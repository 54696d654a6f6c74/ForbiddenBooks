using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.DatabaseLogic.Context;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.Exceptions;
using System.IO;
using System;
using ForbiddenBooks.CLI.Utils;

/*TODO
 - creating unit tests for remove exceptions
*/

namespace ForbiddenBooksUnitTests
{
    public class Tests
    {
        [TestCase]
        public void BasicTest()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                .UseInMemoryDatabase(databaseName: "BasicTestDB")
                .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User { Id = 1, FirstName = "Stokata" });
                dc.AddEntity(new User { Id = 2, FirstName = "Enizkata" });

                Assert.AreEqual(2, dc.GetEntries<User>().Count);
            }
        }

        [TestCase]
        public void AddingEntityToDatabaseWithoutExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                .UseInMemoryDatabase(databaseName: "AddingEntityToDatabaseWithoutExceptionsDB")
                .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User { Id = 1, FirstName = "first", LastName = "last" });
                dc.AddEntity(new Genre { Id = 1, Name = "name" });

                Assert.AreEqual("first", dc.GetEntries<User>()[0].FirstName);
                Assert.AreEqual("name", dc.GetEntries<Genre>()[0].Name);
            }
        }

        [TestCase]
        public void AddEntityCheckItemException()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "AddEntityCheckItemExceptionDB")
                            .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                var input = new StringReader("");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                dc.AddEntity(new Genre { Id = 1, Name = "g" });
                dc.AddEntity(new Genre { Id = 2, Name = "g" });

                Assert.AreEqual("g is already present in the database\r\n", output.ToString());
            }
        }

        [TestCase]
        public void AddEntityCheckPersonException()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "AddEntityCheckPersonExceptionDB")
                            .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                var input = new StringReader("");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                dc.AddEntity(new User { Id = 1, FirstName = "PakHakvam", LastName = "Nasa" });
                dc.AddEntity(new User { Id = 2, FirstName = "PakHakvam", LastName = "Nasa" });

                Assert.AreEqual("PakHakvam Nasa is already present in the database\r\n", output.ToString());
            }
        }

        [TestCase]
        public void UpdateEntityNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "UpdateEntityNoExceptionsDB")
                            .Options;

            IQueryable<Genre> data = new List<Genre>()
            {
                new Genre{Id = 1, Name = "g1"},
                new Genre{Id = 2, Name = "g2"},
                new Genre{Id = 3, Name = "g3"},
            }.AsQueryable();

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                data.ToList().ForEach(g => dc.AddEntity(g));
                Genre g = dc.GetEntryById<Genre>(1);

                g.Name = "g11";
                Assert.AreEqual("g11", dc.GetEntryById<Genre>(1).Name);
            }
        }

        [TestCase]
        public void UpdateEntityMatchingException()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "UpdateEntityMatchingExceptionDB")
                            .Options;

            IQueryable<Genre> data = new List<Genre>()
            {
                new Genre{Id = 1, Name = "g1"},
                new Genre{Id = 2, Name = "g2"},
                new Genre{Id = 3, Name = "g3"},
            }.AsQueryable();

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                data.ToList().ForEach(g => dc.AddEntity(g));
                Genre g = dc.GetEntryById<Genre>(1);

                var input = new StringReader("");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                g.Name = "g2";
                dc.UpdateEntry(g);

                Assert.AreEqual("g2 is already present in the database\r\n", output.ToString());
            }
        }

        [TestCase]
        public void DelitingNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "DelitingNoExceptionsDB")
                            .Options;

            IQueryable<Genre> data = new List<Genre>()
            {
                new Genre{Id = 1, Name = "g1"},
                new Genre{Id = 2, Name = "g2"},
                new Genre{Id = 3, Name = "g3"},
            }.AsQueryable();

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                data.ToList().ForEach(g => dc.AddEntity(g));
                Genre g = dc.GetEntryById<Genre>(1);

                dc.RemoveEntity(g);
            }
        }

        [TestCase]
        public void CheckIfTableIsEmpty()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "CheckIfTableIsEmptyDB")
                            .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Genre { Id = 1, Name = "g" });
                Assert.AreEqual(false, query.IsTableEmpty<Genre>());
                Assert.AreEqual(true, query.IsTableEmpty<User>());
                Assert.AreEqual(true, query.IsTableEmpty<Author>());
                Assert.AreEqual(true, query.IsTableEmpty<Magazine>());
            }
        }

        [TestCase]
        public void GettingUsersByFirstName()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GettingUsersByFirstNameDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User { Id = 1, FirstName = "f1", LastName = "l1" });
                dc.AddEntity(new User { Id = 2, FirstName = "f1", LastName = "l2" });
                dc.AddEntity(new User { Id = 3, FirstName = "f2", LastName = "l3" });
                Assert.AreEqual(new List<int>() { 1, 2 }, query.GetUsersByFirstName("f1").Select(u => u.Id).ToList());
            }
        }

        [TestCase]
        public void GettingUsersByLastName()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GettingUsersByLastNameDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User { Id = 1, FirstName = "f1", LastName = "l1" });
                dc.AddEntity(new User { Id = 2, FirstName = "f2", LastName = "l1" });
                dc.AddEntity(new User { Id = 3, FirstName = "f3", LastName = "l2" });
                Assert.AreEqual(new List<int>() { 1, 2 }, query.GetUsersByLastName("l1").Select(u => u.Id).ToList());
            }
        }

        [TestCase]
        public void GettingUsersByBothNames()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GettingUsersByBothNamesDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new User { Id = 1, FirstName = "f1", LastName = "l1" });
                dc.AddEntity(new User { Id = 2, FirstName = "f2", LastName = "l2" });
                dc.AddEntity(new User { Id = 3, FirstName = "f3", LastName = "l3" });
                Assert.AreEqual(new List<int>() { 2 }, query.GetUsersByBothNames("f2", "l2").Select(u => u.Id).ToList());
            }
        }

        [TestCase]
        public void GettingAuthorsByFirstName()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GettingAuthorsByFirstNameDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Author { Id = 1, FirstName = "f1", LastName = "l1" });
                dc.AddEntity(new Author { Id = 2, FirstName = "f1", LastName = "l2" });
                dc.AddEntity(new Author { Id = 3, FirstName = "f2", LastName = "l3" });
                Assert.AreEqual(new List<int>() { 1, 2 }, query.GetAuthorsByFirstName("f1").Select(u => u.Id).ToList());
            }
        }

        [TestCase]
        public void GettingAuthorsByLastName()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GettingAuthorsByLastNameDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Author { Id = 1, FirstName = "f1", LastName = "l1" });
                dc.AddEntity(new Author { Id = 2, FirstName = "f2", LastName = "l1" });
                dc.AddEntity(new Author { Id = 3, FirstName = "f3", LastName = "l2" });
                Assert.AreEqual(new List<int>() { 1, 2 }, query.GetAuthorsByLastName("l1").Select(u => u.Id).ToList());
            }
        }

        [TestCase]
        public void GettingAuthorsByBothNames()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "GettingAuthorsByBothNamesDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Author { Id = 1, FirstName = "f1", LastName = "l1" });
                dc.AddEntity(new Author { Id = 2, FirstName = "f2", LastName = "l2" });
                dc.AddEntity(new Author { Id = 3, FirstName = "f3", LastName = "l3" });
                Assert.AreEqual(new List<int>() { 2 }, query.GetAuthorsByBothNames("f2", "l2").Select(u => u.Id).ToList());
            }
        }

        [TestCase]
        public void GettingAllEntities()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "GettingAllEntitiesDB")
                            .Options;

            IQueryable<Genre> data = new List<Genre>()
            {
                new Genre{Id = 1, Name = "g1"},
                new Genre{Id = 2, Name = "g2"},
                new Genre{Id = 3, Name = "g3"},
            }.AsQueryable();

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                data.ToList().ForEach(g => dc.AddEntity(g));
                Assert.AreEqual(data.ToList(), dc.GetEntries<Genre>());
            }
        }

        [TestCase]
        public void GettingMagazinesByName()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "GettingMagazinesByNameDB")
                            .Options;
            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Magazine { Id = 1, Name = "m1" });
                dc.AddEntity(new Magazine { Id = 2, Name = "m2" });
                dc.AddEntity(new Magazine { Id = 3, Name = "m3" });

                Assert.AreEqual(new List<int> { 3 }, query.GetMagazinesByName("m3").Select(m => m.Id).ToList());
            }
        }

        [TestCase]
        public void GettingMarketsByName()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "GettingMarketsByNameDB")
                            .Options;
            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Market { Id = 1, Name = "m1" });
                dc.AddEntity(new Market { Id = 2, Name = "m2" });
                dc.AddEntity(new Market { Id = 3, Name = "m3" });

                Assert.AreEqual(new List<int> { 2 }, query.GetMarketsByName("m2").Select(m => m.Id).ToList());
            }
        }

        [TestCase]
        public void GettingGenresByName()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "GettingGenresByNameDB")
                            .Options;
            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Genre { Id = 1, Name = "g1" });
                dc.AddEntity(new Genre { Id = 2, Name = "g2" });
                dc.AddEntity(new Genre { Id = 3, Name = "g3" });

                Assert.AreEqual(new List<int> { 1 }, query.GetGenresByName("g1").Select(g => g.Id).ToList());
            }
        }

        [TestCase]
        public void GettingMagazinesUnderAccessLevel()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "GettingMagazinesUnderAccessLevelDB")
                            .Options;
            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Magazine { Id = 1, Name = "m1", AccsessLevel = 1 });
                dc.AddEntity(new Magazine { Id = 4, Name = "m2", AccsessLevel = 2 });
                dc.AddEntity(new Magazine { Id = 2, Name = "m3", AccsessLevel = 3 });
                dc.AddEntity(new Magazine { Id = 3, Name = "m4", AccsessLevel = 4 });

                Assert.AreEqual(new List<int> { 1, 4, 2 }, query.GetMagazinesUnderAccessLevel(3).Select(m => m.Id).ToList());
            }
        }

        [TestCase]
        public void GettingMagazinesOverAccessLevel()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "GettingMagazinesOverAccessLevelDB")
                            .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Magazine { Id = 1, Name = "m1", AccsessLevel = 1 });
                dc.AddEntity(new Magazine { Id = 4, Name = "m2", AccsessLevel = 2 });
                dc.AddEntity(new Magazine { Id = 2, Name = "m3", AccsessLevel = 3 });
                dc.AddEntity(new Magazine { Id = 3, Name = "m4", AccsessLevel = 4 });

                Assert.AreEqual(new List<int> { 3 }, query.GetMagazinesOverAccessLevel(3).Select(m => m.Id).ToList());
            }
        }

        [TestCase]
        public void UserBuyingMagazineInsufficientFunds()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "UserBuyingMagazineInsufficientFundsDB")
                            .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Market { Id = 1, Name = "cheren" });
                dc.AddEntity(new Magazine { Id = 1, Name = "m1", AccsessLevel = 1, Price = 10 });
                dc.AddEntity(new User { Id = 1, FirstName = "stokata", LastName = "malinin", AcessLevel = 10, Balance = 1 });

                try
                {
                    query.UserBuyMagazine(dc.GetEntryById<User>(1), dc.GetEntryById<Magazine>(1), dc.GetEntryById<Market>(1));
                }
                catch (InsufficientFunds e)
                {
                    Assert.Pass();
                    return;
                }
            }

            Assert.Fail();
        }

        [TestCase]
        public void UserBuyingMagazineInsufficientAccessLevel()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                            .UseInMemoryDatabase(databaseName: "        public void UserBuyingMagazineInsufficientAccessLevelDB")
                            .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Market { Id = 1, Name = "cheren" });
                dc.AddEntity(new Magazine { Id = 1, Name = "m1", AccsessLevel = 10, Price = 1 });
                dc.AddEntity(new User { Id = 1, FirstName = "stokata", LastName = "malinin", AcessLevel = 1, Balance = 10 });

                try
                {
                    query.UserBuyMagazine(dc.GetEntryById<User>(1), dc.GetEntryById<Magazine>(1), dc.GetEntryById<Market>(1));
                }
                catch (InsufficientAccessLevel e)
                {
                    Assert.Pass();
                    return;
                }
            }

            Assert.Fail();
        }

        [TestCase]
        public void FindItemEntityNoExceptions()
        {
            var options = new DbContextOptionsBuilder<ForbiddenBooksContext>()
                           .UseInMemoryDatabase(databaseName: "FindItemEntityNoExceptionsDB")
                           .Options;

            using (ForbiddenBooksContext context = new ForbiddenBooksContext(options))
            {
                DataController dc = new DataController(context);
                DbQuery query = new DbQuery(dc);

                dc.AddEntity(new Genre() { Id = 1, Name = "genre1" });

                var input = new StringReader("stokata\ngenre1");
                Console.SetIn(input);

                var output = new StringWriter();
                Console.SetOut(output);

                Genre genreFound = GenericUtil.FindEntity<Genre>(dc);
                Genre genreOriginal = dc.GetEntryById<Genre>(1);

                Assert.AreEqual(genreFound.Id, genreOriginal.Id);
                Assert.AreEqual(genreFound.Name, genreOriginal.Name);
                Assert.AreNotEqual(genreFound, genreOriginal);
            }
        }
    }
}