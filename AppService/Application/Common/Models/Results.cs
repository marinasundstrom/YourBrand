using System;

namespace YourBrand.Application.Common.Models;

public record class Results<T>(IEnumerable<T> Items, int TotalCount);