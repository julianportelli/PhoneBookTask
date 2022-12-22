using Microsoft.EntityFrameworkCore;
using PhoneBook.API.Database;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;

namespace PhoneBook.API.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly PhoneBookDbContext _phoneBookDbContext;

        public CompanyRepository(PhoneBookDbContext phoneBookDbContext)
        {
            _phoneBookDbContext = phoneBookDbContext;
        }

        public async Task<CompanyRetrieveDTO> CreateCompanyAsync(string name, DateTime registrationDate)
        {
            using (var transaction = _phoneBookDbContext.Database.BeginTransaction())
            {
                try
                {
                    var company = new Company
                    {
                        Name = name,
                        RegistrationDate = registrationDate,
                    };

                    _phoneBookDbContext.Companies.Add(company);
                    await _phoneBookDbContext.SaveChangesAsync();
                    transaction.Commit();

                    var companyRetrieveDTO = new CompanyRetrieveDTO
                    {
                        Id = company.Id,
                        Name = company.Name,
                        RegistrationDate = company.RegistrationDate
                    };

                    return companyRetrieveDTO;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<IEnumerable<CompanyRetrieveDTO>> GetAllCompaniesWithLinkedPersonsCountAsync()
        {
            List<CompanyRetrieveDTO> companyWithLinkedPersons;

            companyWithLinkedPersons = await _phoneBookDbContext.Companies.Include(c => c.Persons).Select(c => new CompanyRetrieveDTO
            {
                Id = c.Id,
                Name = c.Name,
                RegistrationDate = c.RegistrationDate,
                Persons = c.Persons.Select(p => new PersonCompanyRetrieveDTO
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber
                }).ToList(),
                NoOfPersonsLinked = c.Persons.Count()
            }).ToListAsync();

            return companyWithLinkedPersons;
        }

        public bool DoesCompanyNameAlreadyExist(string name)
        {
            return _phoneBookDbContext.Companies.Where(x => x.Name == name).FirstOrDefault() != null;
        }
    }
}
