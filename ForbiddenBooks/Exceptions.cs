using System;
using System.Collections.Generic;
using System.Linq;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.DatabaseLogic.Tables.Base;

namespace ForbiddenBooks.Exceptions
{
    public class EntityNotFoundInDatabase : Exception
    {
        public EntityNotFoundInDatabase(string message = "Entry is not present in the database") : base(message) { }
    }

    public class EntryDependsOnDeletedItem : Exception
    {
        private static string GenerateMessage(DatabaseLogic.DataController dc, Person person)
        {
            List<Magazine> l = dc.GetEntries<Magazine>().Where(m => m.userOwner == person || m.Author == person).ToList();
            string message = $"\nCannot delete {person.FirstName} {person.LastName}\nDependees:\n";
            foreach (Magazine m in l) message += $"{m.Id} {m.Name}\n";

            return message;
        }

        private static string GenerateMessage(DatabaseLogic.DataController dc, Item item)
        {
            List<Magazine> l = dc.GetEntries<Magazine>().Where(m => m.Genre == item || m.marketOwner == item).ToList();
            string message = $"\nCannot delete {item.Name}\nDependees:\n";
            foreach (Magazine m in l) message += $"{m.Id} {m.Name}\n";

            return message;
        }

        public EntryDependsOnDeletedItem(DatabaseLogic.DataController dc, Item item) : base(GenerateMessage(dc, item)) { }

        public EntryDependsOnDeletedItem(DatabaseLogic.DataController dc, Person person) : base(GenerateMessage(dc, person)) { }
    }

    public class InsufficientAccessLevel : Exception
    {
        public InsufficientAccessLevel() { }

        public InsufficientAccessLevel(string message) : base(message) { }
    }

    public class MagazineNotOwned : Exception
    {
        public MagazineNotOwned() { }

        public MagazineNotOwned(string message) : base(message) { }
    }

    public class InsufficientFunds : Exception
    {
        public InsufficientFunds() { }

        public InsufficientFunds(string message) : base(message) { }
    }

    public class UserAlreadyOwnsMagazine : Exception
    {
        public UserAlreadyOwnsMagazine() { }

        public UserAlreadyOwnsMagazine(string message) : base(message) { }
    }

    public class MagazineNotPresentInMarket : Exception
    {
        public MagazineNotPresentInMarket() { }

        public MagazineNotPresentInMarket(string message) : base(message) { }
    }

    public class AlreadyOnThatMarket : Exception
    {
        public AlreadyOnThatMarket() { }

        public AlreadyOnThatMarket(string message) : base(message) { }
    }

    public class MagazineNotOwnedByUser : Exception
    {
        public MagazineNotOwnedByUser() { }

        public MagazineNotOwnedByUser(string message) : base(message) { }
    }

    public class MagazineAlreadyOwnedByAUser : Exception
    {
        public MagazineAlreadyOwnedByAUser() { }

        public MagazineAlreadyOwnedByAUser(string message) : base(message) { }
    }

    public class EntityAlreadyContainedInDatabase : Exception
    {
        public EntityAlreadyContainedInDatabase() { }

        public EntityAlreadyContainedInDatabase(string message) : base(message) { }
    }

    public class InvalidCommand : Exception
    {
        public InvalidCommand() { }

        public InvalidCommand(string message) : base(message) { }
    }
}
