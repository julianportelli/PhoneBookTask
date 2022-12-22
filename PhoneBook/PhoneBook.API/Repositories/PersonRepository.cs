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
                CompanyId = p.CompanyId
            }).ToListAsync();

            return persons;
        }

        public async Task<bool> DoesCompanyExistAsync(int id)
        {
            return await _phoneBookDbContext.Companies.AnyAsync(x => x.Id == id);
        }

        private async Task<bool> DoesPersonExistAsync(int id)
        {
            return await _phoneBookDbContext.Persons.AnyAsync(x => x.Id == id);
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

        public async Task<PersonAddUpdateResultDTO> CreateUpdateDeletePersonAsync(PersonAddUpdateDTO personAddUpdateDTO, DbActionTypeEnum dbActionType)
        {
            using (var transaction = _phoneBookDbContext.Database.BeginTransaction())
            {
                string nonExistantPersonErrorMessage = $"Person with {nameof(Person.Id)} {personAddUpdateDTO.Id} does not exist";
                string nonExistantCompanyErrorMessage = $"Company with {nameof(Company.Id)} {personAddUpdateDTO.CompanyId} does not exist";
                bool changesMade = true;

                try
                {
                    switch (dbActionType)
                    {
                        case DbActionTypeEnum.Delete:
                            if (!await DoesPersonExistAsync(personAddUpdateDTO.Id))
                            {
                                throw new ArgumentException(nonExistantPersonErrorMessage);
                            }

                            _phoneBookDbContext.Remove(personAddUpdateDTO.Id);
                            break;
                        case DbActionTypeEnum.Add:
                            if (!await DoesCompanyExistAsync(personAddUpdateDTO.CompanyId))
                            {
                                throw new ArgumentException(nonExistantCompanyErrorMessage);
                            }

                            var personToAdd = new Person
                            {
                                FullName = personAddUpdateDTO.FullName,
                                PhoneNumber = personAddUpdateDTO.PhoneNumber,
                                Address = personAddUpdateDTO.Address,
                                CompanyId = personAddUpdateDTO.CompanyId
                            };

                            _phoneBookDbContext.Persons.Add(personToAdd);
                            break;
                        case DbActionTypeEnum.Update:
                            var personToUpdate = await _phoneBookDbContext.Persons.FindAsync(personAddUpdateDTO.Id);

                            if(personToUpdate == null)
                            {
                                throw new ArgumentException(nonExistantPersonErrorMessage);
                            }

                            if (!await DoesCompanyExistAsync(personAddUpdateDTO.CompanyId))
                            {
                                throw new ArgumentException(nonExistantCompanyErrorMessage);
                            }

                            personToUpdate.FullName = personAddUpdateDTO.FullName;
                            personToUpdate.PhoneNumber = personAddUpdateDTO.PhoneNumber;
                            personToUpdate.Address = personAddUpdateDTO.Address;
                            personToUpdate.CompanyId = personAddUpdateDTO.CompanyId;

                            _phoneBookDbContext.Persons.Update(personToUpdate);
                            break;
                        default:
                            changesMade = false;
                            break;
                    }

                    if (changesMade)
                    {
                        await _phoneBookDbContext.SaveChangesAsync();
                        transaction.Commit();
                    }

                    return new PersonAddUpdateResultDTO
                    {
                        PersonAddUpdateDTO = personAddUpdateDTO,
                        ChangesMade = changesMade
                    };
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
