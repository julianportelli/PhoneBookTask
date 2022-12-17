using PhoneBook.API.Models;

namespace PhoneBook.API.Services
{
    public interface IPersonService
    {
        Task<Person> CreatePersonAsync(string name, string phoneNumber, string address, int companyId);
        Task<Person> GetPersonByIdAsync(int id);
        Task<IEnumerable<Person>> GetAllPersonsAsync();
        Task<IEnumerable<Person>> SearchPersonsByFieldsAsync(string name, string phoneNumber, string address, string companyName);
        Task<Person> UpdatePersonAsyc(Person person);
        Task<bool> DeletePersonAsync(Person person);
        Task<Person> GetRandomPersonAsync();
    }
}
