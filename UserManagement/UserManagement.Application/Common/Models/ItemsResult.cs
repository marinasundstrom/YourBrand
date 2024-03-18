using System;

namespace YourBrand.UserManagement.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);