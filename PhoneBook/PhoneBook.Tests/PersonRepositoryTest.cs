using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Tests
{
    public class PersonRepositoryTest : PhonebookDbFixture
    {
        [Fact]
        public void SearchPersonsByFieldsAsync_Returns_List_Containing_All_Person_Details_If_PersonSearchDTO_Is_Empty()
        {
            var phoneBookDbContextMock = new DbContextMock<PhoneBookDbContext>(fixtureDbContextOptions);
            //Uses package EntityFrameworkCoreMock.Moq
            var companiesDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Companies, _dbSetCompany.Object);
            var personsDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Persons, _dbSetPerson.Object);

            var sut = new PersonRepository(phoneBookDbContextMock.Object);

            var searchDTO = new PersonSearchDTO();

            var result = (List<PersonRetrieveDTO>)sut.SearchPersonsByFieldsAsync(searchDTO).Result;

            result.Should().NotBeNull();
            result.Count.Should().Be(_dbSetPerson.Object.Count());
        }

        [Fact]
        public void SearchPersonsByFieldsAsync_Returns_List_Containing_Persons_Details_If_PersonSearchDTO_Contains_Company_Name()
        {
            var phoneBookDbContextMock = new DbContextMock<PhoneBookDbContext>(fixtureDbContextOptions);
            //Uses package EntityFrameworkCoreMock.Moq
            var companiesDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Companies, _dbSetCompany.Object);
            var personsDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Persons, _dbSetPerson.Object);

            var sut = new PersonRepository(phoneBookDbContextMock.Object);

            var searchDTO = new PersonSearchDTO
            {
                CompanyName = _dbSetCompany.Object.First().Name
            };

            var result = (List<PersonRetrieveDTO>)sut.SearchPersonsByFieldsAsync(searchDTO).Result;

            result.Should().NotBeNull();
            result.Count.Should().Be(_dbSetCompany.Object.Count()); //There are x persons for each company where x is the number of companies
        }

        [Fact]
        public void SearchPersonsByFieldsAsync_Returns_Empty_List_If_PersonSearchDTO_Details_Do_Not_Match_Any_Records()
        {
            var phoneBookDbContextMock = new DbContextMock<PhoneBookDbContext>(fixtureDbContextOptions);
            //Uses package EntityFrameworkCoreMock.Moq
            var companiesDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Companies, _dbSetCompany.Object);
            var personsDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Persons, _dbSetPerson.Object);

            var sut = new PersonRepository(phoneBookDbContextMock.Object);

            var searchDTO = new PersonSearchDTO
            {
                CompanyName = "???",
                Address = "???",
                FullName = "???",
                PhoneNumber = "???"
            };

            var result = (List<PersonRetrieveDTO>)sut.SearchPersonsByFieldsAsync(searchDTO).Result;

            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public void CreatePersonAsync_Adds_New_Record()
        {
            /*var phoneBookDbContextMock = new DbContextMock<PhoneBookDbContext>(fixtureDbContextOptions);
            //Uses package EntityFrameworkCoreMock.Moq
            var companiesDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Companies, _dbSetCompany.Object);
            var personsDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Persons, _dbSetPerson.Object);

            var sut = new PersonRepository(phoneBookDbContextMock.Object);

            var personCountBeforeAdd = phoneBookDbContextMock.Object.Persons.Count();
            var company = _dbSetCompany.Object.First();
            var result = sut.CreatePersonAsync("John", "99999999", "Marsa", company.Id).Result;

            result.Should().NotBeNull();
            result.Company.Should().BeEquivalentTo(company);
            Assert.True(phoneBookDbContextMock.Object.Persons.Count() == personCountBeforeAdd + 1);*/

            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var personCountBeforeAdd = fixtureInMemoryDbContext.Persons.Count();
            var company = fixtureInMemoryDbContext.Companies.First();

            var result = sut.CreatePersonAsync("John", "99999999", "Marsa", company.Id).Result;
            result.Should().NotBeNull();
            Assert.True(result.Company.Name == company.Name);
            Assert.True(fixtureInMemoryDbContext.Persons.Count() == personCountBeforeAdd + 1);
        }

        [Fact]
        public async void CreatePersonAsync_Throws_Exception_When_Exception_Thrown()
        {
            var phoneBookDbContextMock = new DbContextMock<PhoneBookDbContext>(fixtureDbContextOptions);
            //Uses package EntityFrameworkCoreMock.Moq
            var companiesDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Companies, _dbSetCompany.Object);
            var personsDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Persons, _dbSetPerson.Object);

            phoneBookDbContextMock.Setup(s => s.SaveChangesAsync(default))
            .Throws(new Exception());

            var sut = new PersonRepository(phoneBookDbContextMock.Object);

            await Assert.ThrowsAsync<Exception>(() => sut.CreatePersonAsync("?", "?", "?", -1));
        }

        [Fact]
        public void DoesCompanyExistAsync_Given_Valid_CompanyId_Returns_True()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var company = fixtureInMemoryDbContext.Companies.First();

            var result = sut.DoesCompanyExistAsync(company.Id).Result;
            result.Should().Be(true);
        }

        [Fact]
        public void DoesCompanyExistAsync_Given_Invalid_CompanyId_Returns_False()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);

            var result = sut.DoesCompanyExistAsync(-1).Result;
            result.Should().Be(false);
        }

        [Fact]
        public void GetRandomPersonAsync_Gets_Random_Person()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);

            for (int i = 0; i < fixtureInMemoryDbContext.Persons.Count(); i++)
            {
                var result = sut.GetRandomPersonAsync().Result;
                Assert.True(fixtureInMemoryDbContext.Persons.Select(x => x.Id).Contains(result.Id));
                Assert.True(fixtureInMemoryDbContext.Companies.Select(x => x.Id).Contains(result.Company.Id));
            }
        }
    }
}
