namespace PhoneBook.API.Models.DTOs
{
    public class CompanyRetrieveDTO
    {
        public Company? Company { get; set; }
        public int NoOfPersonsLinked { get; set; } = 0;
    }
}
