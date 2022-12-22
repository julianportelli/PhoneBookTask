namespace PhoneBook.API.Models.DTOs
{
    public class PersonCompanyRetrieveDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CompanyRetrieveDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public List<PersonCompanyRetrieveDTO> Persons { get; set; } = new ();
        public int NoOfPersonsLinked { get; set; } = 0;
    }
}
