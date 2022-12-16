namespace PhoneBook.API.Models.DTOs
{
    public class CompanyDTO
    {
        public Company? Company { get; set; }
        public uint NoOfPersonsLinked { get; set; } = 0;
    }
}
