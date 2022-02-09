using System;
using System.Security.Claims;

using Catalog.Application.Common.Interfaces;
using Catalog.Infrastructure;

namespace Catalog.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public string? GetAccessToken() => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
}