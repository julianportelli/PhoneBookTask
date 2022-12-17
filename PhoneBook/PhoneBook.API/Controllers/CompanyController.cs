using Microsoft.AspNetCore.Mvc;
using PhoneBook.API.Models.DTOs;
using PhoneBook.API.Repositories;

namespace PhoneBook.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyController(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpPut]
    [Route("Add")]
    public async Task<IActionResult> Add([FromBody] CompanyAddDTO companyAddDTO)
    {
        try
        {
            var formattedCompanyName = companyAddDTO.CompanyName.Trim();

            if (_companyRepository.DoesCompanyNameAlreadyExist(formattedCompanyName))
            {
                return BadRequest($"Company with name \"{formattedCompanyName}\" already exists.");
            }

            var companyRegistrationDateAsDate = DateTime.Parse(companyAddDTO.RegistrationDate);


            var company = await _companyRepository.CreateCompanyAsync(formattedCompanyName, companyRegistrationDateAsDate);

            if (company == null)
            {
                throw new Exception("An error occured while inserting a new company");
            }

            return Ok(company);
        }
        catch (FormatException fe)
        {
            return BadRequest($"Invalid {nameof(CompanyAddDTO.RegistrationDate)} provided. Value provided was {companyAddDTO.RegistrationDate}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}