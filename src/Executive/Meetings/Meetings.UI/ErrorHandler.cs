using MudBlazor;

namespace YourBrand.Meetings;

public class ErrorHandler(ISnackbar snackbar) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return base.SendAsync(request, cancellationToken);
        }
        catch (ApiException<ProblemDetails> exc)
        {
            snackbar.Add(exc.Result.Title, Severity.Error);

            throw;
        }
    }
}