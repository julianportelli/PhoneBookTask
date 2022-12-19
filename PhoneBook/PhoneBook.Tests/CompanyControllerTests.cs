
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;
using PhoneBook.API.Controllers;
using PhoneBook.API.Models.DTOs;

namespace PhoneBook.Tests;

public class CompanyControllerTests : IClassFixture<DatabaseFixture>
{
    private CompanyAddDTO _validCompanyAddDTO1 = new()
    {
        CompanyName = "Google",
        RegistrationDate = "1999-04-02"
    };

    private CompanyAddDTO _validCompanyAddDTO2 = new()
    {
        CompanyName = "Amazon",
        RegistrationDate = "1994-07-05"
    };

    private CompanyAddDTO _validCompanyAddDTO2CopyButWithSpaces = new()
    {
        CompanyName = " Amazon ",
        RegistrationDate = "1994-07-05"
    };

    private CompanyAddDTO _invalidDateCompanyAddDTO = new()
    {
        CompanyName = "InvalidDate",
        RegistrationDate = "2022-31-31"
    };

    private CompanyController _sut;
    private CompanyRepository _companyRepo;
    private DatabaseFixture _fixture;

    public CompanyControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _companyRepo = new CompanyRepository(fixture.inMemoryDbContext);
        _sut = new CompanyController(_companyRepo);
    }

    [Fact]
    public void Add_Company_Given_Valid_Object_Should_Return_StatusCode_200_And_Should_Return_Company_With_Id()
    {
        var result = (OkObjectResult)_sut.Add(_validCompanyAddDTO1).Result;

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeOfType<Company>().Which.Id.Should().NotBe(0);
    }

    [Fact]
    public void Add_Company_Given_Object_With_Invalid_Date_Should_Return_StatusCode_400_And_Include_Date_In_Value()
    {
        var result = (ObjectResult)_sut.Add(_invalidDateCompanyAddDTO).Result;

        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Value.ToString().Should().Contain(_invalidDateCompanyAddDTO.RegistrationDate.ToString());
    }

    [Fact]
    public void Add_Company_When_Exception_Thrown_Should_Return_StatusCode_500()
    {
        var mockCompanyRepository = new Mock<ICompanyRepository>();
        mockCompanyRepository.Setup(s => s.CreateCompanyAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .Throws(new Exception());

        var companyControllerWithMock = new CompanyController(mockCompanyRepository.Object);

        var result = (ObjectResult)companyControllerWithMock.Add(_validCompanyAddDTO1).Result;

        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void Add_Company_When_CreateCompanyAsync_Returns_NullShould_Return_StatusCode_500()
    {
        var mockCompanyRepository = new Mock<ICompanyRepository>();

        //Bypass unique company name rule
        mockCompanyRepository.Setup(s => s.DoesCompanyNameAlreadyExist(It.IsAny<string>()))
        .Returns(false);

        //Return null from CreateCompanyAsync
        mockCompanyRepository.Setup(s => s.CreateCompanyAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync((Company)null);

        var companyControllerWithMock = new CompanyController(mockCompanyRepository.Object);

        var result = (ObjectResult)companyControllerWithMock.Add(_validCompanyAddDTO1).Result;

        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Value.Should().BeOfType<string>();
    }

    [Fact]
    public void Add_Company_Company_With_Same_Name_And_Even_With_Spaces_Should_Return_StatusCode_400()
    {
        var result1 = (ObjectResult)_sut.Add(_validCompanyAddDTO2).Result;

        result1.StatusCode.Should().Be(StatusCodes.Status200OK);
        result1.Value.Should().BeOfType<Company>().Which.Id.Should().NotBe(0);

        var result2 = (ObjectResult)_sut.Add(_validCompanyAddDTO2).Result;

        result2.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result2.Value.Should().BeOfType<string>();

        var result3 = (ObjectResult)_sut.Add(_validCompanyAddDTO2CopyButWithSpaces).Result;

        result3.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result3.Value.Should().BeOfType<string>();
    }

    [Fact]
    public void GetALl_Should_Not_Be_Null_And_Return_StatusCode_200()
    {
        var result1 = (ObjectResult)_sut.GetAll().Result;

        result1.StatusCode.Should().Be(StatusCodes.Status200OK);
        result1.Value.Should().NotBeNull();
        result1.Value.Should().BeOfType<List<CompanyRetrieveDTO>>();
    }

    [Fact]
    public void GetALl_Add_Company_Should_Retrieve_Company_With_Id()
    {
        var companyDTOToAdd = new CompanyAddDTO
        {
            CompanyName = "Test",
            RegistrationDate = "2022-12-19"
        };

        var addResult = (ObjectResult)_sut.Add(companyDTOToAdd).Result;
        var company = (Company)addResult.Value;

        var result = (ObjectResult)_sut.GetAll().Result;
        var companyRetrieveDTOList = (List<CompanyRetrieveDTO>)result.Value;
        Assert.Contains(company.Id, companyRetrieveDTOList.Select(x => x.Company.Id));
    }

    [Fact]
    public void GetAll_When_Exception_Thrown_Should_Return_StatusCode_500()
    {
        var mockCompanyRepository = new Mock<ICompanyRepository>();
        mockCompanyRepository.Setup(s => s.GetAllCompaniesWithLinkedPersonsCountAsync())
            .Throws(new Exception());

        var companyControllerWithMock = new CompanyController(mockCompanyRepository.Object);

        var result = (ObjectResult)companyControllerWithMock.GetAll().Result;

        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}