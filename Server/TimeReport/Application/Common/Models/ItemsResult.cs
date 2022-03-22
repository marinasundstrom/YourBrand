using System;

namespace YourBrand.TimeReport.Application.Common.Models;

public record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);