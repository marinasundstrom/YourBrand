namespace BlazorApp;

public sealed class RequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Method => _httpContextAccessor.HttpContext!.Request.Method;
}

public static class RequestContextExtensions
{
    public static bool IsGet(this RequestContext requestContext) => requestContext.Method == "GET";

    public static bool IsPost(this RequestContext requestContext) => requestContext.Method == "POST";

    public static bool IsPut(this RequestContext requestContext) => requestContext.Method == "PUT";

    public static bool IsDelete(this RequestContext requestContext) => requestContext.Method == "DELETE";
}