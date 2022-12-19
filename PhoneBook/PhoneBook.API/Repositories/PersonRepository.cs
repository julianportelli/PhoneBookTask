using Microsoft.EntityFrameworkCore;
using PhoneBook.API.Constants.Enums;
using PhoneBook.API.Database;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;
using System.ComponentModel.Design;
using System.Net;

namespace PhoneBook.API.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly PhoneBookDbContext _phoneBookDbContext;

        public PersonRepository(PhoneBookDbContext phoneBookDbContext)
        {
            _phoneBookDbContext = phoneBookDbContext;
        }

        public async Task<Person> CreatePersonAsync(string fullName, string phoneNumber, string address, int companyId)
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

                    return person;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<IEnumerable<PersonRetrieveDTO>> GetAllPersonsAsync()
        {
            var allPersons = await _phoneBookDbContext.Persons.ToListAsync();
            var allCompanies = await _phoneBookDbContext.Companies.ToListAsync();

            var personsRetrieveDTOList = new List<PersonRetrieveDTO>();
            allPersons.ForEach(person =>
            {
                personsRetrieveDTOList.Add(new PersonRetrieveDTO
                {
                    Person = person,
                    Company = allCompanies.Where(x => x.Id == person.CompanyId).FirstOrDefault()
                });
            });

            return personsRetrieveDTOList;
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

        public Task<Person> GetRandomPersonAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Person>> SearchPersonsByFieldsAsync(string name, string phoneNumber, string address, string companyName)
        {
            throw new NotImplementedException();
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
