using System;

namespace YourBrand.Marketing.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);