namespace BlazorApp.BankId;

internal sealed class FakeBankIdService : IBankIdService
{
    private static int StatusCallCount = 0;
    private const int NrOfCallsBeforeComplete = 2;
    private static readonly bool LockOnStarted = false;

    public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default)
    {
        StatusCallCount = 0;
        return Task.FromResult(new AuthenticateResponse(FakeConstants.FakeReferenceToken, AutoStartToken: FakeConstants.FakeAutoStartToken));
    }

    public Task<GetStatusResponse> GetStatusAsync(GetStatusRequest request, CancellationToken cancellationToken = default)
    {
        if(request.ReferenceToken != FakeConstants.FakeReferenceToken)
        {
            return Task.FromResult(new GetStatusResponse(BankIdStatus.Error));
        }
 
        if(LockOnStarted) 
        {
            return Task.FromResult(new GetStatusResponse(BankIdStatus.Started));
        }

        if(StatusCallCount < NrOfCallsBeforeComplete) 
        {
            StatusCallCount++;
            return Task.FromResult(new GetStatusResponse(BankIdStatus.OutstandingTransaction));
        }
        else if (StatusCallCount == NrOfCallsBeforeComplete)
        {
            StatusCallCount++;
            return Task.FromResult(new GetStatusResponse(BankIdStatus.Started, QrCode: FakeConstants.FakeQrCode));
        }
        else if (StatusCallCount == NrOfCallsBeforeComplete + 1)
        {
            StatusCallCount++;
            return Task.FromResult(new GetStatusResponse(BankIdStatus.UserSign));
        }

        return Task.FromResult(new GetStatusResponse(BankIdStatus.Complete, FakeConstants.Name, FakeConstants.GivenName, FakeConstants.Surname, FakeConstants.Ssn));
    }
}

internal class FakeConstants
{
    public static readonly string FakeReferenceToken = "1337";
    internal static readonly string? FakeAutoStartToken = "42";
    internal static readonly string? FakeQrCode = "data:image/png;base64,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";
    internal static readonly string? Name = "John Smith";
    internal static readonly string? GivenName = "John";
    internal static readonly string? Surname = "Smith";
    internal static readonly string? Ssn = "19730628-3423";
}