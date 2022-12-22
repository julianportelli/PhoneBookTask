namespace PhoneBook.API.Models.DTOs
{
    public class CompanyPersonRetrieveDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }

    public class PersonRetrieveDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public CompanyPersonRetrieveDTO Company { get; set; }
    }
}
