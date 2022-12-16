using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;

namespace PhoneBook.API.Repositories
{
    public interface ICompanyService
    {
        Task<Company> CreateCompanyAsync(string name, DateTime registrationDate);
        Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync();
    }
}
