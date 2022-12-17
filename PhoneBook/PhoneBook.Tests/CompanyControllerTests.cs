using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using PhoneBook.API.Controllers;
using PhoneBook.API.Database;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;
using PhoneBook.API.Repositories;
using System.Net;

namespace PhoneBook.Tests;

public class CompanyControllerTests
{
    private CompanyAddDTO _validCompanyAddDTO;
    private CompanyAddDTO _invalidDateCompanyAddDTO;
    private CompanyController _sut;
    private CompanyRepository _companyRepo;

    public CompanyControllerTests()
    {
        _validCompanyAddDTO = new()
        {
            CompanyName = "Google",
            RegistrationDate = "1999-04-02"
        };

        _invalidDateCompanyAddDTO = new()
        {
            CompanyName = "InvalidDate",
            RegistrationDate = "2022-31-31"
        };

        var options = new DbContextOptionsBuilder<PhoneBookDbContext>()
            .UseInMemoryDatabase(databaseName: "PhoneBookDB")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))  //Avoids raising error that in-memory db doesn't support transactions
            .Options;

        var inMemoryDbContext = new PhoneBookDbContext(options);
        _companyRepo = new CompanyRepository(inMemoryDbContext);
        _sut = new CompanyController(_companyRepo);
    }

    [Fact]
    public void Add_Company_Given_Valid_Object_Should_Return_StatusCode_200()
    {
        var result = (OkObjectResult)_sut.Add(_validCompanyAddDTO).Result;

        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public void Add_Company_Given_Valid_Object_Should_Return_Company_With_Id()
    {
        var result = (OkObjectResult)_sut.Add(_validCompanyAddDTO).Result;

        result.Value.Should().BeOfType<Company>()
            .Which.Id.Should().NotBe(0);
    }

    [Fact]
    public void Add_Company_When_Exception_Thrown_Should_Return_StatusCode_500()
    {
        var mockCompanyRepository = new Mock<ICompanyRepository>();
        mockCompanyRepository.Setup(s => s.CreateCompanyAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .Throws(new Exception());

        var companyControllerWithMock = new CompanyController(mockCompanyRepository.Object);

        var result = (ObjectResult)companyControllerWithMock.Add(_validCompanyAddDTO).Result;

        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}