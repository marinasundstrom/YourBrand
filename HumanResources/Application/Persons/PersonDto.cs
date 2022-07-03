namespace YourBrand.HumanResources.Application.Persons;

public record class PersonDto(string Id, string FirstName, string LastName, string? DisplayName, string Role, string SSN, string Email, DepartmentDto? Department, DateTime Created, DateTime? LastModified);
