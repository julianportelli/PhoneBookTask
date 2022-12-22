using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.API.Constants.Enums;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;
using PhoneBook.API.Repositories;
using PhoneBook.API.Helpers;
using System.ComponentModel.Design;
using System;

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
        public async Task<IActionResult> Add([FromBody] PersonAddUpdateDTO personAddDTO)
        {
            try
            {
                if (!await _personRepository.DoesCompanyExistAsync(personAddDTO.CompanyId))
                {
                    return BadRequest($"Company with {nameof(PersonAddUpdateDTO.CompanyId)} {personAddDTO.CompanyId} does not exist");
                }

                var person = await _personRepository.CreatePersonAsync(personAddDTO.FullName, personAddDTO.PhoneNumber, personAddDTO.Address, personAddDTO.CompanyId);

                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("WildCard")]
        public async Task<IActionResult> GetRandomPerson()
        {
            try
            {
                var persons = await _personRepository.GetRandomPersonAsync();

                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("AddEditRemove/{dbAction}")]
        public async Task<IActionResult> AddEditRemove([FromBody] PersonAddUpdateDTO person, string dbAction)
        {
            try
            {
                var actionEnum = EnumHelpers.ParseEnumCustom<DbActionTypeEnum>(dbAction);

                var result = await _personRepository.CreateUpdateDeletePersonAsync(person, actionEnum);

                return Ok(result);
            }
            catch (ArgumentException ex){
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> Search([FromBody] PersonSearchDTO personSearchDTO)
        {
            try
            {
                var persons = await _personRepository.SearchPersonsByFieldsAsync(personSearchDTO);

                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
