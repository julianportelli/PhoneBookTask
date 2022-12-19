using PhoneBook.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Tests
{
    public class PersonControllerTests : IClassFixture<DatabaseFixture>
    {
        private PersonAddDTO _validPersonAddDTO1 = new()
        {
            FullName = "Joshua",
            Address = "Manikata",
            PhoneNumber = "12345",
            CompanyId = 1
        };

        private PersonAddDTO _invalidDatePersonAddDTO = new()
        {
            FullName = "Joshua",
            Address = "Manikata",
            PhoneNumber = "12345",
            CompanyId = 0
        };

        private PersonController _sut;
        private PersonRepository _companyRepo;
        private DatabaseFixture _fixture;

        public PersonControllerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _companyRepo = new PersonRepository(fixture.inMemoryDbContext);
            _sut = new PersonController(_companyRepo);
        }

    }
}
