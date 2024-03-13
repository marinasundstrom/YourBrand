using System;

namespace YourBrand.Analytics.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);