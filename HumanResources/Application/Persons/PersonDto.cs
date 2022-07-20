using YourBrand.HumanResources.Application.Organizations;

namespace YourBrand.HumanResources.Application.Persons;

public record class PersonDto(string Id, OrganizationDto Organization, string FirstName, string LastName, string? DisplayName, string Title, string Role, string SSN, string Email, DepartmentDto? Department, Person2Dto? ReportsTo, DateTime Created, DateTime? LastModified);

public record class Person2Dto(string Id, OrganizationDto Organization, string FirstName, string LastName, string? DisplayName, string Title, DepartmentDto? Department);

