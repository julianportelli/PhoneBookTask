using PhoneBook.API.Constants.Enums;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;

namespace PhoneBook.API.Repositories
{
    public interface IPersonRepository
    {
        Task<Person> CreatePersonAsync(string name, string phoneNumber, string address, int companyId);
        Task<Person> CreateUpdateDeletePersonAsync(Person person, DbActionTypeEnum dbActionType);
        Task<Person> GetPersonByIdAsync(int id);
        Task<Company> GetCompanyByIdAsync(int id);
        Task<bool> DoesCompanyExistAsync(int id);
        Task<IEnumerable<PersonRetrieveDTO>> GetAllPersonsAsync();
        Task<IEnumerable<Person>> SearchPersonsByFieldsAsync(string name, string phoneNumber, string address, string companyName);
        Task<Person> GetRandomPersonAsync();
    }
}
