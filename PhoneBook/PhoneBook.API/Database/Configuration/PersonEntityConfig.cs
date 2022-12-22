using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhoneBook.API.Models;

namespace PhoneBook.API.Database.Configuration
{
    public class PersonEntityConfig : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person");
            builder.HasKey(p => p.Id);
            builder
                .HasOne(p => p.Company)
                .WithMany(c => c.Persons)
                .HasForeignKey(p => p.CompanyId)
                .IsRequired();
        }
    }
}
