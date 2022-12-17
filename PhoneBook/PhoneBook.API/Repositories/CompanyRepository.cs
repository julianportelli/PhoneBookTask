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

        public async Task<Company> CreateCompanyAsync(string name, DateTime registrationDate)
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

                    _phoneBookDbContext.Add(company);
                    await _phoneBookDbContext.SaveChangesAsync();
                    transaction.Commit();

                    return company;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        private Company GetCompanyByName(string name)
        {
            return _phoneBookDbContext.Companies.Where(x => x.Name == name).FirstOrDefault();
        }

        public Task<IEnumerable<CompanyRetrieveDTO>> GetAllCompaniesAsync()
        {
            throw new NotImplementedException();
        }

        public bool DoesCompanyNameAlreadyExist(string name)
        {
            return GetCompanyByName(name) != null;
        }
    }
}
