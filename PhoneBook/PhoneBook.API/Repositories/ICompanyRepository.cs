﻿using PhoneBook.API.Models;
using PhoneBook.API.Models.DTOs;

namespace PhoneBook.API.Repositories
{
    public interface ICompanyRepository
    {
        Task<CompanyRetrieveDTO> CreateCompanyAsync(string name, DateTime registrationDate);
        bool DoesCompanyNameAlreadyExist(string name);
        Task<IEnumerable<CompanyRetrieveDTO>> GetAllCompaniesWithLinkedPersonsCountAsync();
    }
}
