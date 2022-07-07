using System;

namespace YourBrand.Invoicing.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);