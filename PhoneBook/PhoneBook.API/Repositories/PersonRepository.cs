using Microsoft.EntityFrameworkCore;
using PhoneBook.API.Constants.Enums;
using PhoneBook.API.Database;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace PhoneBook.API.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly PhoneBookDbContext _phoneBookDbContext;

        public PersonRepository(PhoneBookDbContext phoneBookDbContext)
        {
            _phoneBookDbContext = phoneBookDbContext;
        }

        public async Task<PersonRetrieveDTO> CreatePersonAsync(string fullName, string phoneNumber, string address, int companyId)
        {
            using (var transaction = _phoneBookDbContext.Database.BeginTransaction())
            {
                try
                {
                    var person = new Person
                    {
                        FullName = fullName,
                        PhoneNumber = phoneNumber,
                        Address = address,
                        CompanyId = companyId
                    };

                    _phoneBookDbContext.Persons.Add(person);
                    await _phoneBookDbContext.SaveChangesAsync();
                    transaction.Commit();

                    var personRetrieveDTO = _phoneBookDbContext.Persons.Include(p => p.Company).Where(p => p.Id == person.Id)
                        .Select(p => new PersonRetrieveDTO
                        {
                            Id = p.Id,
                            FullName = p.FullName,
                            PhoneNumber = p.PhoneNumber,
                            Address = p.Address,
                            Company = new CompanyPersonRetrieveDTO
                            {
                                Id = p.Company.Id,
                                Name = p.Company.Name,
                                RegistrationDate = p.Company.RegistrationDate,
                            }
                        })
                        .FirstOrDefault();

                    return personRetrieveDTO;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<IEnumerable<PersonBasicRetrieveDTO>> GetAllPersonsAsync()
        {
            var persons = await _phoneBookDbContext.Persons.Select(p => new PersonBasicRetrieveDTO
            {
                Id = p.Id,
                FullName = p.FullName,
                Address = p.Address,
                PhoneNumber = p.PhoneNumber,
            }).ToListAsync();

            return persons;
        }

        public async Task<bool> DoesCompanyExistAsync(int id)
        {
            return await _phoneBookDbContext.Companies.AnyAsync(x => x.Id == id);
        }
        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await _phoneBookDbContext.Companies.FindAsync(id);
        }

        public Task<Person> GetPersonByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PersonRetrieveDTO> GetRandomPersonAsync()
        {
            var personIds = _phoneBookDbContext.Persons.Select(x => x.Id).ToList();
            var random = new Random();
            int randomPersonIndex = random.Next(0, personIds.Count);

            var person = await _phoneBookDbContext.Persons
                .Include(p => p.Company)
                .Where(p => p.Id == personIds[randomPersonIndex])
                .Select(p => new PersonRetrieveDTO
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber,
                    Address = p.Address,
                    Company = new CompanyPersonRetrieveDTO
                    {
                        Id = p.Company.Id,
                        Name = p.Company.Name,
                        RegistrationDate = p.Company.RegistrationDate,
                    }
                }).FirstOrDefaultAsync();

            return person;
        }

        //WIP
        public async Task<IEnumerable<PersonRetrieveDTO>> SearchPersonsByFieldsAsync(PersonSearchDTO personSearchDTO)
        {
            var persons = await _phoneBookDbContext.Persons
                .Include(p => p.Company)
                .Where(p => p.FullName.Contains(personSearchDTO.FullName))
                .Where(p => p.PhoneNumber.Contains(personSearchDTO.PhoneNumber))
                .Where(p => p.Address.Contains(personSearchDTO.Address))
                .Where(p => p.Company.Name.Contains(personSearchDTO.CompanyName))
                .Select(p => new PersonRetrieveDTO
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber,
                    Address = p.Address,
                    Company = new CompanyPersonRetrieveDTO
                    {
                        Id = p.Company.Id,
                        Name = p.Company.Name,
                        RegistrationDate = p.Company.RegistrationDate,
                    }
                })
                .ToListAsync();

            return persons;
        }

        public async Task<Person> CreateUpdateDeletePersonAsync(Person person, DbActionTypeEnum dbActionType)
        {
            using (var transaction = _phoneBookDbContext.Database.BeginTransaction())
            {
                try
                {
                    switch (dbActionType)
                    {
                        case DbActionTypeEnum.Delete:
                            _phoneBookDbContext.Remove(person.Id);
                            break;
                        case DbActionTypeEnum.Insert:
                            person = new Person
                            {
                                FullName = person.FullName,
                                PhoneNumber = person.PhoneNumber,
                                Address = person.Address,
                                CompanyId = person.CompanyId
                            };

                            _phoneBookDbContext.Persons.Add(person);
                            break;
                        case DbActionTypeEnum.Update:
                            _phoneBookDbContext.Persons.Update(person);
                            break;
                        default:
                            break;
                    }

                    await _phoneBookDbContext.SaveChangesAsync();
                    transaction.Commit();

                    return person;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
