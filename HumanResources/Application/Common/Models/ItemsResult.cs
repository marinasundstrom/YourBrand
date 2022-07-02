using System;

namespace YourBrand.HumanResources.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);