namespace PhoneBook.API.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int CompanyId { get; set; }
    }
}
