namespace PhoneBook.API.Models.DTOs
{
    public class PersonAddUpdateDeleteDTO
    {
        public int Id { get; set; } = 0;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int CompanyId { get; set; } = 0;
    }

    public class PersonAddUpdateDeleteResultDTO
    {
        public PersonAddUpdateDeleteDTO PersonAddUpdateDTO { get; set; }
        public bool ChangesMade { get; set; }
    }
}
