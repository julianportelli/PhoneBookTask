using Microsoft.EntityFrameworkCore;
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
    }
}
