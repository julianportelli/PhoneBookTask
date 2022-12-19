using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.API.Constants.Enums;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;
using PhoneBook.API.Repositories;

namespace PhoneBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private IPersonRepository _personRepository;

        public PersonController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [HttpPut]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] PersonAddDTO personAddDTO)
        {
            try
            {
                if (!await _personRepository.DoesCompanyExistAsync(personAddDTO.CompanyId))
                {
                    return BadRequest($"Company with {nameof(PersonAddDTO.CompanyId)} does not exist");
                }

                var person = await _personRepository.CreatePersonAsync(personAddDTO.FullName, personAddDTO.PhoneNumber, personAddDTO.Address, personAddDTO.CompanyId);

                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException);
            }
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var persons = await _personRepository.GetAllPersonsAsync();

                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException);
            }
        }

        [HttpPost]
        [Route("AddEditRemove/{dbAction}")]
        public async Task<IActionResult> AddEditRemove([FromBody] Person person, string dbAction)
        {
            try
            {
                var actionEnum = ToEnum<DbActionTypeEnum>(dbAction);

                var result = await _personRepository.CreateUpdateDeletePersonAsync(person, actionEnum);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private T ToEnum<T>(string enumString)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), enumString);
            }
            catch (Exception ex)
            {
                string enumValues = string.Join(", ", Enum.GetNames(typeof(T)));

                throw new ArgumentException($"The action \"{enumString}\" is not supported. Only {enumValues} are supported.");
            }
        }
    }
}
