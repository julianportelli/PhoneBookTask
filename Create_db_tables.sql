use test_db;

CREATE TABLE IF NOT EXISTS Company(
Id int AUTO_INCREMENT PRIMARY KEY,
Name varchar(255) UNIQUE,
RegistrationDate DateTime 
);

CREATE TABLE IF NOT EXISTS Person (
Id int AUTO_INCREMENT PRIMARY KEY,
FullName varchar(255),
PhoneNumber varchar(255), -- Not sure what max phone number length can actually be
Address varchar(255),
CompanyId int,
FOREIGN KEY (CompanyId) 
	references Company (Id) 
	ON UPDATE RESTRICT ON DELETE CASCADE
);