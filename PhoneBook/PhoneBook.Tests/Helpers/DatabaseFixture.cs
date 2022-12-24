using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Tests.Helpers
{
    public class PhonebookDbFixture : IDisposable
    {
        public DbContextOptions<PhoneBookDbContext> fixtureDbContextOptions;
        public PhoneBookDbContext fixtureInMemoryDbContext;

        public List<Company> companyData;
        public List<Person> personData;

        public Mock<DbSet<Company>> _dbSetCompany;
        public Mock<DbSet<Person>> _dbSetPerson;

        public PhonebookDbFixture()
        {
            var guid = Guid.NewGuid().ToString();
            fixtureDbContextOptions = new DbContextOptionsBuilder<PhoneBookDbContext>()
            .UseInMemoryDatabase(databaseName: guid)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))  //Avoids raising error that in-memory db doesn't support transactions
            .Options;

            fixtureInMemoryDbContext = new PhoneBookDbContext(fixtureDbContextOptions);
            fixtureInMemoryDbContext.Database.EnsureCreated();
            // initialize data in the test database 
            SetupAndSeedDatabase();
        }

        private void SetupAndSeedDatabase()
        {
            companyData = new List<Company>(){
                new Company { Id = 1, Name = "Facebook", RegistrationDate = DateTime.Now },
                new Company { Id = 2, Name = "Apple", RegistrationDate = DateTime.Now },
                new Company { Id = 3, Name = "Amazon", RegistrationDate = DateTime.Now },
                new Company { Id = 4, Name = "Netflix", RegistrationDate = DateTime.Now },
                new Company { Id = 5, Name = "Google", RegistrationDate = DateTime.Now },
            };

            fixtureInMemoryDbContext.Companies.AddRange(companyData);

            personData = new List<Person>();

            for (int i = 0; i < companyData.Count; i++)
            {
                var comp = companyData[i];
                for (int j = 1; j <= companyData.Count; j++)
                {
                    var personId = j + (i * companyData.Count);
                    personData.Add(new Person
                    {
                        Id = personId,
                        FullName = $"Person{personId}Company{comp.Id}",
                        PhoneNumber = personId.ToString(),
                        Address = $"Address{personId}{comp.Id}",
                        CompanyId = comp.Id,
                        Company = comp
                    });
                }
            }

            fixtureInMemoryDbContext.Persons.AddRange(personData);
            fixtureInMemoryDbContext.SaveChanges();

            //https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN

            var companyDataQueryable = companyData.AsQueryable();

            var mockSetCompany = new Mock<DbSet<Company>>();
            mockSetCompany.As<IQueryable<Company>>().Setup(m => m.Provider).Returns(companyDataQueryable.Provider);
            mockSetCompany.As<IQueryable<Company>>().Setup(m => m.Expression).Returns(companyDataQueryable.Expression);
            mockSetCompany.As<IQueryable<Company>>().Setup(m => m.ElementType).Returns(companyDataQueryable.ElementType);
            mockSetCompany.As<IQueryable<Company>>().Setup(m => m.GetEnumerator()).Returns(() => companyDataQueryable.GetEnumerator());

            var personDataQueryable = personData.AsQueryable();

            var mockSetPerson = new Mock<DbSet<Person>>();
            mockSetPerson.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(personDataQueryable.Provider);
            mockSetPerson.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDataQueryable.Expression);
            mockSetPerson.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDataQueryable.ElementType);
            mockSetPerson.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(() => personDataQueryable.GetEnumerator());

            _dbSetCompany = mockSetCompany;
            _dbSetPerson = mockSetPerson;
        }

        public void Dispose()
        {
            // clean up test data from the database
            fixtureInMemoryDbContext.Database.EnsureDeleted();
        }
    }
}
