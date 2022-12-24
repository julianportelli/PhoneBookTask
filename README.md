# phone_book_task_helio

This is my submission for the coding task given to me by Helio Gaming.

There are two projects under the PhoneBook directory. One is PhoneBook.API, which contains the actual implementation of the API, and the other is PhoneBook.Tests, which contains unit tests for the API.

The MySql Database Dump folder contains the dump for the MySql database used for testing. It contains some data already.
The Postman Export folder contains the postman collection which I used to test the API manualy. You can use this to test the API yourself.

To get started, open PhoneBook/PhoneBook.sln in Visual Studio. You probably will need to restore NuGet packages.

- Target framework: net6.0
- Database used: MySql
- Dependency injection for controller dependencies was used using AddTransient method in Program.cs
- ORM: Entity Framework Core
- Unit test packages: XUnit (for creating unit tests), Moq + EntityFrameworkCoreMock.Moq (for mocking interfaces and dbcontext), FluentAssertions (for cleaner assertions (sometimes)), EntityFrameworkCore.InMemory (for in-memory database testing)
- Unit tests cover all code except Program.cs

## Phone Book Task
Implement an application in C# \ .Net to manage the following two entities: companies and persons.

Company
- Add
  - Company Name (string) (Unique)
  - Registration Date (date)
- View: display / retrieve all
  - Also calculate number of persons linked to the company
Person
- Add
  - Full Name (string)
  - Phone number (string)
  - Address (string)
  - Company (drop down / link)
- View All: display / retrieve all
  - Search: to check through all fields including Company Name
  - View Profile
    - Edit
    - Remove
- Wild card: opens a random profile’s edit page.

Further instructions:
- Company needs to be stored in a separate table and linked via a foreign key.
- Wild card: random pick functionality should be implemented in backend.
- Search: needs to work on all fields including Company.

## Implementation

Applicant can choose to consume the above functionality through one of the below two options.

### Option 1: Web APIs & Unit Tests
- Expose the above functionality as API methods through a ASP.Net Web Application
- Create a Unit Test Project for the following methods Note: inputs can either be randomized or use variables with static values at the start of the test methods
  -  Company_Add
  -  Company_GetAll:
     -  Including “No. People” property
  -  Person_Add
  -  Person_Add_Edit_Remove: add, edit, and remove all in one method
  -  Person_GetAll: retrieve all
  -  Person_Search: retrieve entries matching the search criteria.
  -  Person_WildCard: randomly retrieve a person object.
  
## Deliverables

Provide GIT repo containing
- Code in .Net can be either .Net Framework or .Net 6.0 and a modern Visual Studio solution like 2022 or 2019
- MySQL or SQL Server database \ schema backup
- In case front-end is not in .Net provide separate GIT Repo

## Appendix
If unsure which project templates to use here are some suggestions

.NET Framework
- ASP.Net Web Application (.NET Framework)
- Unit Test Project (.NET Framework)

.NetCore / .Net 6.0
- ASP.NET Core Web API
- MSTest Test Project
