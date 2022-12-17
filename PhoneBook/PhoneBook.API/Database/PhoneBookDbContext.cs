using Microsoft.EntityFrameworkCore;
using PhoneBook.API.Database.Configuration;
using PhoneBook.API.Models;

namespace PhoneBook.API.Database
{
    public class PhoneBookDbContext : DbContext
    {
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Person> Persons => Set<Person>();

        public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new CompanyEntityConfig());
        }
    }
}
