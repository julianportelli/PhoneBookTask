using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;

namespace PhoneBook.API.Services
{
    public interface ICompanyService
    {
        Task<Company> CreateCompanyAsync(string name, DateTime registrationDate);
        bool DoesCompanyNameAlreadyExist(string name);
        Task<IEnumerable<CompanyRetrieveDTO>> GetAllCompaniesAsync();
    }
}
