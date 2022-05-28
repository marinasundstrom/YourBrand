using System;

namespace YourBrand.Payments.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);