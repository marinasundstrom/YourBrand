using IdentityModel.Client;

namespace BlazorApp.BankId;

public interface IBankIdService 
{
    public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default);

    public Task<GetStatusResponse> GetStatusAsync(GetStatusRequest request, CancellationToken cancellationToken = default);
}

public record AuthenticateRequest(string? Ssn, string? Message = null);

public record AuthenticateResponse(string? ReferenceToken, string? AutoStartToken);

public record GetStatusRequest(string? ReferenceToken);

public record GetStatusResponse(BankIdStatus Status, string? Name = null, string? GivenName = null, string? Surname = null, string? Ssn = null, string? QrCode = null);

public enum BankIdStatus
{
    OutstandingTransaction,
    NoClient,
    Started,
    UserSign,
    UserReq,
    Complete,
    Error
}
