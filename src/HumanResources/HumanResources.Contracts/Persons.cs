namespace YourBrand.HumanResources.Contracts;

public record CreatePerson
{
    public string OrganizationId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? DisplayName { get; init; }
    public string Title { get; init; }
    public string Role { get; init; }
    public string SSN { get; init; }
    public string Email { get; init; }
    public string DepartmentId { get; init; }
    public string? ReportsTo { get; init; }
    public string Password { get; init; }
};

public record CreatePersonResponse(string PersonId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string SSN, string Email);

public record PersonCreated(string PersonId, string CreatedById);

public record PersonUpdated(string PersonId, string UpdatedById);

public record PersonDeleted(string PersonId, string DeletedById);

public record GetPerson(string PersonId, string RequestedById);

public record GetPersonResponse(string PersonId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string SSN, string Email);