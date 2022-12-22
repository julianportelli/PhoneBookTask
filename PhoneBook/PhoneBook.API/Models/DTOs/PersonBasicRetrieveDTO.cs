﻿namespace PhoneBook.API.Models.DTOs
{
    public class PersonBasicRetrieveDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int CompanyId { get; set; }
    }
}
