using System;

namespace YourBrand.Invoices.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);