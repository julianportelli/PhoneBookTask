using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Tests.Helpers
{
    public class DatabaseFixture : IDisposable
    {

        public DbContextOptions<PhoneBookDbContext> options;
        public PhoneBookDbContext inMemoryDbContext;

        public DatabaseFixture()
        {
            var guid = Guid.NewGuid().ToString();
            options = new DbContextOptionsBuilder<PhoneBookDbContext>()
            .UseInMemoryDatabase(databaseName: guid)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))  //Avoids raising error that in-memory db doesn't support transactions
            .Options;

            inMemoryDbContext = new PhoneBookDbContext(options);

            // initialize data in the test database 
        }

        public void Dispose()
        {
            // clean up test data from the database
            inMemoryDbContext.Database.EnsureDeleted();
        }
    }
}
