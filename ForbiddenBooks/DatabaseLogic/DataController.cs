using System.Linq;
using ForbiddenBooks.Exceptions;
using System.Collections.Generic;
using ForbiddenBooks.DatabaseLogic.Tables;
using Microsoft.EntityFrameworkCore;
using ForbiddenBooks.DatabaseLogic.Tables.Interfaces;
using ForbiddenBooks.DatabaseLogic.Tables.Base;

namespace ForbiddenBooks.DatabaseLogic
{
    /// <summary>
    /// The so called "Buisness Logic"
    /// </summary>
    public class DataController
    {
        public readonly Context.ForbiddenBooksContext context;

        public DataController()
        {
            context = new Context.ForbiddenBooksContext();
        }
        public DataController(Context.ForbiddenBooksContext context)
        {
            this.context = context;
        }

        ~DataController()
        {
            context.Dispose();
        }

        // Create --------------------------------------------------------

        /// <summary>
        /// Adds an entity into the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void AddEntity<T>(T entity) where T : class
        {
            try
            {
                CheckIfEntityAlreadyExists(entity);
                context.GetTable<T>().Add(entity);
                context.SaveChanges();
            }
            catch (System.Exception e) { System.Console.WriteLine(e.Message); }
        }

        // Read ---------------------------------------------------------

        /// <summary>
        /// Gets an entity from the database via type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetEntries<T>() where T: class
        {
            return context.GetTable<T>().ToList();
        }

        /// <summary>
        /// Gets an entity from the database via ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetEntryById<T>(int id) where T: class
        {
            return context.GetTable<T>().Find(id);
        }

        private void CheckIfEntityAlreadyExists<T>(T entity) where T : class
        {
            if (typeof(T).BaseType == typeof(Item))
            {
                if (GetEntries<T>().
                    Any(x => (x as IEntity).Id != (entity as IEntity).Id &&
                    (x as Item).Name == (entity as Item).Name) == true)
                    throw new EntityAlreadyContainedInDatabase((entity as Item).Name + " is already present in the database");
            }
            else if(typeof(T).BaseType == typeof(Person))
            {
                if (GetEntries<T>().
                    Any(x => (x as IEntity).Id != (entity as IEntity).Id &&
                    (x as Person).FirstName == (entity as Person).FirstName
                    && (x as Person).LastName == (entity as Person).LastName) == true)
                    throw new EntityAlreadyContainedInDatabase((entity as Person).FirstName + " " + (entity as Person).LastName + " is already present in the database");
            }
        }
        // Update ------------------------------------------------------

        /// <summary>
        /// Updates an entity in the database where the passed param's ID matches the target's ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void UpdateEntry<T>(T entity) where T : class
        {
            T item = context.GetTable<T>().Find((entity as IEntity).Id);
            if (item == null) throw new EntityNotFoundInDatabase();

            try
            {
                CheckIfEntityAlreadyExists(entity);
                context.Entry(item).CurrentValues.SetValues(entity);
                context.SaveChanges();
            }
            catch(EntityAlreadyContainedInDatabase e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        // Delete -----------------------------------------------------

        private void Remove<T>(T entity)
        {
            context.Remove(entity);
            context.SaveChanges();
        }

        private void RemoveRelatedByCollection<T>(T entity) where T : IRelatedByCollection
        {
            if (entity.Magazines.Count > 0)
            {
                if (entity.GetType().BaseType == typeof(User))
                    throw new EntryDependsOnDeletedItem(this, entity as User);
                else if (entity.GetType().BaseType == typeof(Market))
                    throw new EntryDependsOnDeletedItem(this, entity as Market);
            }
            Remove(entity);
        }


        private void RemoveRelatedByQuery<T>(T entity) where T : IRelatedByQuery
        {
            if (entity.GetType().BaseType == typeof(Item))
            {
                if (GetEntries<Magazine>().Any(m => (m.Genre as Genre).Id == (entity as Genre).Id == true))
                {
                    throw new EntryDependsOnDeletedItem(this, entity as Genre);
                }
            }
            else if (entity.GetType().BaseType == typeof(Person))
            {
                if (GetEntries<Magazine>().Any(m => (m.Author as Author).Id == (entity as Author).Id == true))
                {
                    throw new EntryDependsOnDeletedItem(this, entity as Author);
                }
            }

            Remove(entity);
        }

        /// <summary>
        /// Deletes a specified entity from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void RemoveEntity<T>(T entity) where T : class
        {
            try
            {
                T item = context.GetTable<T>().Find((entity as IEntity).Id);
                if (item == null) throw new EntityNotFoundInDatabase();

                if (typeof(IRelatedByCollection).IsAssignableFrom(typeof(T)))
                {
                    RemoveRelatedByCollection(item as IRelatedByCollection);
                    return;
                }
                else if (typeof(IRelatedByQuery).IsAssignableFrom(typeof(T)))
                {
                    RemoveRelatedByQuery(item as IRelatedByQuery);
                    return;
                }
                Remove(item);
            }
            catch (System.Exception e) { System.Console.WriteLine(e.Message); }
        }

        //Testing --------------------------------------

        /// <summary>
        /// Clear all entries from the database and reset all ID counters
        /// </summary>
        public void ResetDB()
        {
            foreach (Magazine x in GetEntries<Magazine>())
                RemoveEntity(x);

            foreach (Genre x in GetEntries<Genre>())
                RemoveEntity(x);

            foreach (Author x in GetEntries<Author>())
                RemoveEntity(x);

            foreach (User u in GetEntries<User>())
                RemoveEntity(u);

            foreach (Market m in GetEntries<Market>())
                RemoveEntity(m);

            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Users', RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Genres', RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Markets', RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Authors', RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Magazines', RESEED, 0)");
        }
    }
}