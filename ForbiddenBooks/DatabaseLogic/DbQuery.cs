using System.Collections.Generic;
using ForbiddenBooks.Exceptions;
using ForbiddenBooks.DatabaseLogic.Tables;
using System.Linq;

namespace ForbiddenBooks.DatabaseLogic
{
    public class DbQuery
    {
        private DataController dc;

        public DbQuery(DataController dc)
        {
            this.dc = dc;
        }

        // Logic --------------------------------------------------------

        /// <summary>
        /// Puts a specified magazine on a specified market
        /// </summary>
        /// <param name="magazine"></param>
        /// <param name="market"></param>
        public void SellMagazineOnMarket(Magazine magazine, Market market)
        {
            if (magazine.marketOwner == market) throw new AlreadyOnThatMarket();
            if (magazine.userOwner != null) throw new MagazineAlreadyOwnedByAUser();

            market.Magazines.Add(magazine);
            dc.UpdateEntry(market);
        }

        /// <summary>
        /// Removes a specified magazine from a specifed market and adds it to a specified user
        /// as long as the user has the necessary access level and balance
        /// </summary>
        /// <param name="buyer"></param>
        /// <param name="item"></param>
        /// <param name="market"></param>
        public void UserBuyMagazine(User buyer, Magazine item, Market market)
        {
            if (buyer.Balance < item.Price) throw new InsufficientFunds();
            if (buyer.AcessLevel < item.AccsessLevel) throw new InsufficientAccessLevel();
            if (buyer.Magazines.Contains(item) == true) throw new UserAlreadyOwnsMagazine();
            if (market.Magazines.Contains(item) == false) throw new MagazineNotPresentInMarket();

            buyer.Balance -= item.Price;

            buyer.Magazines.Add(item);
            market.Magazines.Remove(item);

            dc.UpdateEntry(buyer);
            dc.UpdateEntry(market);
            dc.UpdateEntry(item);
        }

        /// <summary>
        /// Removes a specified magazine from a specified user and adds it to a specified market.
        /// Also refunds the cost of the magazine to the user.
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="item"></param>
        /// <param name="market"></param>
        public void UserSellMagazine(User seller, Magazine item, Market market)
        {
            if (seller.Magazines.Contains(item) == false) throw new MagazineNotOwned();

            seller.Balance += item.Price;

            market.Magazines.Add(item);
            seller.Magazines.Remove(item);

            dc.UpdateEntry(seller);
            dc.UpdateEntry(market);
            dc.UpdateEntry(item);
        }

        // Queries ------------------------------------------------------

        public bool IsTableEmpty<T>() where T: class
        {
            if (dc.context.GetTable<T>().Any() == true) return false;
            return true;
        }

        public List<User> GetUsersByFirstName(string firstName)
        {
            return dc.GetEntries<User>().Where(u => u.FirstName == firstName).ToList();
        }

        public List<User> GetUsersByLastName(string lastName)
        {
            return dc.GetEntries<User>().Where(u => u.LastName == lastName).ToList();
        }

        public List<User> GetUsersByBothNames(string firstName, string lastName)
        {
            return dc.GetEntries<User>().Where(u => u.FirstName == firstName && u.LastName == lastName).ToList();
        }

        public List<Magazine> GetMagazinesByName(string name)
        {
            return dc.GetEntries<Magazine>().Where(m => m.Name == name).ToList();
        }

        public List<Author> GetAuthorsByFirstName(string firstName)
        {
            return dc.GetEntries<Author>().Where(a => a.FirstName == firstName).ToList();
        }

        public List<Author> GetAuthorsByLastName(string lastName)
        {
            return dc.GetEntries<Author>().Where(a => a.LastName == lastName).ToList();
        }

        public List<Author> GetAuthorsByBothNames(string firstName, string lastName)
        {
            return dc.GetEntries<Author>().Where(a => a.FirstName == firstName && a.LastName == lastName).ToList();
        }

        public List<Market> GetMarketsByName(string name)
        {
            return dc.GetEntries<Market>().Where(m => m.Name == name).ToList();
        }

        public List<Genre> GetGenresByName(string name)
        {
            return dc.GetEntries<Genre>().Where(g => g.Name == name).ToList();
        }

        public List<Magazine> GetMagazinesUnderAccessLevel(int accessLevel)
        {
            return dc.GetEntries<Magazine>().Where(m => m.AccsessLevel <= accessLevel).ToList();
        }

        public List<Magazine> GetMagazinesOverAccessLevel(int accessLevel)
        {
            return dc.GetEntries<Magazine>().Where(m => m.AccsessLevel > accessLevel).ToList();
        }

        public List<Magazine> GetAccessibleMagazines(User u)
        {
            return dc.GetEntries<Magazine>().
            Where(m => m.userOwner == null && m.marketOwner != null 
            && m.AccsessLevel <= u.AcessLevel && m.Price <= u.Balance).ToList();
        }

        public List<Magazine> GetNotOwnedMagazines()
        {
            return dc.GetEntries<Magazine>().
            Where(m => m.marketOwner == null && m.userOwner == null).ToList();
        }
    }
}
