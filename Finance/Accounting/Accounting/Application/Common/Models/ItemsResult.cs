using System;

namespace YourBrand.Accounting.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);