using Microsoft.EntityFrameworkCore;
using PhoneBook.API.Database;
using PhoneBook.API.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureDependencies(builder.Services);

ConfigureDatabase(builder.Services);

// Add services to the container.

builder.Services.AddControllers();
    //.AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureDependencies(IServiceCollection services)
{
    //Inject dependencies
    services.AddTransient<ICompanyRepository, CompanyRepository>();
    services.AddTransient<IPersonRepository, PersonRepository>();
}

void ConfigureDatabase(IServiceCollection services)
{
    services.AddDbContext<PhoneBookDbContext>(dbContextOptions =>
    {
        var connectionString = builder.Configuration.GetConnectionString("MySqlDBConnection");
        dbContextOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });
}