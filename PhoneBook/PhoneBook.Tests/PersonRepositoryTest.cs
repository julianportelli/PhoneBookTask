using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneBook.API.Constants.Enums;
using PhoneBook.API.Models.DTOs;

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

        [Fact]
        public void CreateUpdateDeletePersonAsync_Given_Valid_DTO_And_New_Creates_New_Record()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var personCount = fixtureInMemoryDbContext.Persons.Count();
            var company = fixtureInMemoryDbContext.Companies.First();

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO
            {
                Id = 0,
                FullName = "Joseph",
                PhoneNumber = "98989898",
                Address = "Marsalforn",
                CompanyId = company.Id
            };

            var result = sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Add).Result;

            result.PersonAddUpdateDTO.Should().NotBeNull();
            result.PersonAddUpdateDTO.Id.Should().NotBe(0);
            result.ChangesMade.Should().Be(true);
            Assert.True(fixtureInMemoryDbContext.Persons.Count() == personCount + 1);
        }

        [Fact]
        public async void CreateUpdateDeletePersonAsync_Given_Invalid_CompanyId_Throws_AgrumentException()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO
            {
                Id = 0,
                FullName = "Joseph",
                PhoneNumber = "98989898",
                Address = "Marsalforn",
                CompanyId = -1
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Add));
            Assert.Contains(personAddUpdateDTO.CompanyId.ToString(), exception.Message);
        }

        [Fact]
        public void CreateUpdateDeletePersonAsync_Update_Given_Valid_DTO_Is_Succesful()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var personCount = fixtureInMemoryDbContext.Persons.Count();
            var otherCompany = fixtureInMemoryDbContext.Companies.Last();
            var originalPerson = fixtureInMemoryDbContext.Persons.First();
            var originalPersonId = originalPerson.Id;

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO
            {
                Id = originalPerson.Id,
                FullName = originalPerson.FullName,
                PhoneNumber = originalPerson.PhoneNumber,
                Address = originalPerson.Address,
                CompanyId = otherCompany.Id
            };

            var result = sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Update).Result;

            result.PersonAddUpdateDTO.Should().NotBeNull();
            result.PersonAddUpdateDTO.CompanyId.Should().NotBe(originalPersonId);
            result.ChangesMade.Should().Be(true);
        }

        [Fact]
        public async void CreateUpdateDeletePersonAsync_Update_Given_Invalid_PersonId_Throws_ArgumentException()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var personCount = fixtureInMemoryDbContext.Persons.Count();
            var otherCompany = fixtureInMemoryDbContext.Companies.Last();
            var originalPerson = fixtureInMemoryDbContext.Persons.First();
            var originalPersonId = originalPerson.Id;

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO
            {
                Id = -1,
                FullName = "",
                PhoneNumber = "",
                Address = "",
                CompanyId = otherCompany.Id
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Update));
            Assert.Contains(personAddUpdateDTO.Id.ToString(), exception.Message);
        }

        [Fact]
        public async void CreateUpdateDeletePersonAsync_Update_Given_Invalid_CompanyId_Throws_ArgumentException()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var personCount = fixtureInMemoryDbContext.Persons.Count();
            var otherCompany = fixtureInMemoryDbContext.Companies.Last();
            var originalPerson = fixtureInMemoryDbContext.Persons.First();
            var originalPersonId = originalPerson.Id;

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO
            {
                Id = originalPersonId,
                FullName = "",
                PhoneNumber = "",
                Address = "",
                CompanyId = -1
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Update));
            Assert.Contains(personAddUpdateDTO.CompanyId.ToString(), exception.Message);
        }

        [Fact]
        public async void CreateUpdateDeletePersonAsync_Delete_Given_Invalid_PersonId_Throws_ArgumentException()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO
            {
                Id = -1,
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Delete));
            Assert.Contains(personAddUpdateDTO.Id.ToString(), exception.Message);
        }

        [Fact]
        public async void CreateUpdateDeletePersonAsync_Delete_Given_Valid_PersonId_Removes_Record()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var originalPerson = fixtureInMemoryDbContext.Persons.First();

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO
            {
                Id = originalPerson.Id
            };

            var result = await sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Delete);

            var persons = fixtureInMemoryDbContext.Persons.ToList();
            Assert.DoesNotContain(originalPerson.Id, persons.Select(x => x.Id).ToList());
            Assert.True(result.ChangesMade);
        }

        [Fact]
        public async void CreateUpdateDeletePersonAsync_OtherEnum_Does_Not_Make_Changes()
        {
            var sut = new PersonRepository(fixtureInMemoryDbContext);
            var originalPerson = fixtureInMemoryDbContext.Persons.First();

            var personAddUpdateDTO = new PersonAddUpdateDeleteDTO();

            var result = await sut.CreateUpdateDeletePersonAsync(personAddUpdateDTO, DbActionTypeEnum.Other);

            Assert.False(result.ChangesMade);
        }

        [Fact]
        public async void CreateUpdateDeletePersonAsync_Throws_Exception_When_Exception_Thrown()
        {
            var phoneBookDbContextMock = new DbContextMock<PhoneBookDbContext>(fixtureDbContextOptions);
            //Uses package EntityFrameworkCoreMock.Moq
            var companiesDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Companies, _dbSetCompany.Object);
            var personsDbSetMock = phoneBookDbContextMock.CreateDbSetMock(x => x.Persons, _dbSetPerson.Object);
            var company = fixtureInMemoryDbContext.Companies.First();

            phoneBookDbContextMock.Setup(s => s.SaveChangesAsync(default))
            .Throws(new Exception());

            var sut = new PersonRepository(phoneBookDbContextMock.Object);

            await Assert.ThrowsAsync<Exception>(() => sut.CreateUpdateDeletePersonAsync(new PersonAddUpdateDeleteDTO { CompanyId = company.Id }, DbActionTypeEnum.Add));
        }
    }
}
