using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Tests
{
    public class CompanyRepositoryTests : PhonebookDbFixture
    {
        [Fact]
        public async void CreateCompanyAsync_Should_Return_Null_If_Exception_Encountered()
        {
            var phoneBookDbContextMock = new DbContextMock<PhoneBookDbContext>(fixtureDbContextOptions);
            //Uses package EntityFrameworkCoreMock.Moq
            var companiesDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Companies, _dbSetCompany.Object);
            var personsDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Persons, _dbSetPerson.Object);

            phoneBookDbContextMock.Setup(s => s.SaveChangesAsync(default))
                .Throws(new Exception());

            var sut = new CompanyRepository(phoneBookDbContextMock.Object);
            await Assert.ThrowsAsync<Exception>(() => sut.CreateCompanyAsync("Test", DateTime.UtcNow));
        }
    }
}
