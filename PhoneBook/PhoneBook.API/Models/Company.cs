namespace PhoneBook.API.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public virtual List<Person> Persons { get; set; }
    }
}