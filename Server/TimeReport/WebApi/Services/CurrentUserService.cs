using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Domain.Entities;

namespace Skynet.TimeReport.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITimeReportContext _context;
    private string _currentUserId;
    private User _currentUser;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _currentUserId ??= _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? GetAccessToken() => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

    public void SetCurrentUser(string userId)
    {
        if (_currentUserId is not null)
        {
            throw new Exception("User has already been set.");
        }
        _currentUserId = userId;
    }

    public string FirstName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

    public string LastName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
}