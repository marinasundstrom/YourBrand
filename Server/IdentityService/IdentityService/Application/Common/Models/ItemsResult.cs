using System;

namespace YourCompany.IdentityService.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);