using System;

namespace YourBrand.UserManagement.Contracts;

public record CreateUser {
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

public record CreateUserResponse(string UserId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string Email);

public record UserCreated(string UserId, string OrganizationId, string CreatedById);

public record UserUpdated(string UserId, string UpdatedById);

public record UserDeleted(string UserId, string DeletedById);

public record GetUser(string UserId, string RequestedById);

public record GetUserResponse(string UserId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string Email);
