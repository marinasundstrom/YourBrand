namespace Skynet.TimeReport.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
}