using ForbiddenBooks.DatabaseLogic.Tables;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ForbiddenBooks.DatabaseLogic.Context
{
    public class ForbiddenBooksContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<User> Users { get; set; }

        public ForbiddenBooksContext()
        { }

        public ForbiddenBooksContext(DbContextOptions<ForbiddenBooksContext> options) : base(options)
        {  }

        public DbSet<T> GetTable<T>() where T : class
        {
            string name = typeof(T).Name + 's';
            return GetType().
                GetProperty(name).
                GetValue(this) as DbSet<T>;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(@"Server = (localdb)\.; Database = ForbiddenBooksDBv1.0; Integrated Security = true; MultipleActiveResultSets=true;");
            }
        }
    }
}
