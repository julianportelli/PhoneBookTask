using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.API.Constants.Enums;
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
        private PersonAddUpdateDTO _validPersonAddDTO1 = new()
        {
            FullName = "Joshua",
            Address = "Manikata",
            PhoneNumber = "12345",
            CompanyId = 1
        };

        private PersonAddUpdateDTO _invalidDatePersonAddDTO = new()
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

        [Fact]
        public void GetAll_Should_Return_StatusCode_200_And_Type_Should_Be_List_Of_PersonBasicRetrieveDTO()
        {
            var result = (OkObjectResult)_sut.GetAll().Result;

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeOfType<List<PersonBasicRetrieveDTO>>();
        }

        [Fact]
        public void GetAll_On_Exception_Should_Return_StatusCode_500()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.GetAllPersonsAsync()).Throws(new Exception());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.GetAll().Result;

            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeOfType<string>();
        }

        [Fact]
        public void GetRandomPerson_Returns_200()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.GetRandomPersonAsync()).ReturnsAsync(new PersonRetrieveDTO());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.GetRandomPerson().Result;

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeOfType<PersonRetrieveDTO>();
        }

        [Fact]
        public void GetRandomPerson_When_No_Person_Found_Returns_Null()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.GetRandomPersonAsync()).ReturnsAsync(() => null);

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.GetRandomPerson().Result;

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeNull();
        }

        [Fact]
        public void GetRandomPerson_On_Exception_Returns_StatusCode_500()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.GetRandomPersonAsync()).Throws(new Exception());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.GetRandomPerson().Result;

            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeOfType<string>();
        }

        [Fact]
        public void Add_Given_Valid_PersonAddDTO_Returns_StatusCode_200()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.DoesCompanyExistAsync(It.IsAny<int>())).ReturnsAsync(true); //Bypass validation
            mockRepo.Setup(s => s.CreatePersonAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new PersonRetrieveDTO());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.Add(new PersonAddUpdateDTO { CompanyId = 99999 }).Result;

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeOfType<PersonRetrieveDTO>();
        }

        [Fact]
        public void Add_Given_Invalid_Company_Returns_StatusCode_400()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.DoesCompanyExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.Add(new PersonAddUpdateDTO { CompanyId = 99999 }).Result;

            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeOfType<string>().Which.Should().Contain("does not exist");
        }

        [Fact]
        public void Add_On_Exception_Returns_StatusCode_500()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.DoesCompanyExistAsync(It.IsAny<int>())).Throws(new Exception());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.Add(It.IsAny<PersonAddUpdateDTO>()).Result;

            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeOfType<string>();
        }

        [Fact]
        public void AddEditRemove_Returns_StatusCode_200_And_Type_Should_Be_List_Of_PersonRetrieveDTO()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.CreateUpdateDeletePersonAsync(It.IsAny<PersonAddUpdateDTO>(), It.IsAny<DbActionTypeEnum>())).ReturnsAsync(new PersonAddUpdateResultDTO());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.AddEditRemove(new PersonAddUpdateDTO(), "Add").Result;

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeOfType<PersonAddUpdateResultDTO>();
        }

        [Fact]
        public void AddEditRemove_Given_Invalid_DbAction_Returns_400()
        {
            var dbAction = "ABC123";
            var result = (ObjectResult)_sut.AddEditRemove(new PersonAddUpdateDTO(), dbAction).Result;

            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeOfType<string>();
            Assert.Contains(dbAction, result.Value.ToString());
        }

        [Fact]
        public void AddEditRemove_When_Exception_Thrown_Returns_500()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.CreateUpdateDeletePersonAsync(It.IsAny<PersonAddUpdateDTO>(), It.IsAny<DbActionTypeEnum>())).Throws(new Exception());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.AddEditRemove(new PersonAddUpdateDTO(), "Add").Result;

            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeOfType<string>();
        }

        [Fact]
        public void SearchPersonsByFieldsAsync_Returns_StatusCode_200_And_Type_Should_Be_List_Of_PersonRetrieveDTO()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.SearchPersonsByFieldsAsync(It.IsAny<PersonSearchDTO>())).ReturnsAsync(new List<PersonRetrieveDTO>());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.Search(new PersonSearchDTO()).Result;

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().BeOfType<List<PersonRetrieveDTO>>();
        }

        [Fact]
        public void SearchPersonsByFieldsAsync_On_Exception_Returns_StatusCode_500()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(s => s.SearchPersonsByFieldsAsync(It.IsAny<PersonSearchDTO>())).Throws(new Exception());

            var controller = new PersonController(mockRepo.Object);
            var result = (ObjectResult)controller.Search(new PersonSearchDTO()).Result;

            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeOfType<string>();
        }

    }
}
