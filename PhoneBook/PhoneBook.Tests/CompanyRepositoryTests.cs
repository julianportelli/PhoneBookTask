using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Tests
{
    public class CompanyRepositoryTests
    {
        [Fact]
        public void CreateCompanyAsync_Should_Return_Null_If_Exception_Encountered()
        {
            //https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN

            var data = new List<Company>
            {
                new Company { Id = 1, Name = "AAA", RegistrationDate = DateTime.Now },
                new Company { Id = 2,Name = "BBB", RegistrationDate = DateTime.Now },
                new Company { Id = 3,Name = "CCC" , RegistrationDate = DateTime.Now},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Company>>();
            mockSet.As<IQueryable<Company>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Company>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Company>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Company>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var phoneBookDbContextMock = new Mock<PhoneBookDbContext>();
            phoneBookDbContextMock.Setup(c => c.Companies).Returns(mockSet.Object);
            phoneBookDbContextMock.Setup(s => s.SaveChangesAsync(default))
                .Throws(new Exception());

            var sut = new CompanyRepository(phoneBookDbContextMock.Object);
            var result = sut.CreateCompanyAsync("", DateTime.UtcNow);
            result.Should().BeNull();
        }
    }
}
