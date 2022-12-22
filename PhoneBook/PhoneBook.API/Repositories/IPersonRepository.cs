using PhoneBook.API.Constants.Enums;
using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;

namespace PhoneBook.API.Repositories
{
    public interface IPersonRepository
    {
        Task<PersonRetrieveDTO> CreatePersonAsync(string name, string phoneNumber, string address, int companyId);
        Task<PersonAddUpdateResultDTO> CreateUpdateDeletePersonAsync(PersonAddUpdateDTO person, DbActionTypeEnum dbActionType);
        Task<bool> DoesCompanyExistAsync(int id);
        Task<IEnumerable<PersonBasicRetrieveDTO>> GetAllPersonsAsync();
        Task<IEnumerable<PersonRetrieveDTO>> SearchPersonsByFieldsAsync(PersonSearchDTO personSearchDTO);
        Task<PersonRetrieveDTO> GetRandomPersonAsync();
    }
}
