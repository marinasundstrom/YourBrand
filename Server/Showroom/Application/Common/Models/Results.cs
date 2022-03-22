using System;

namespace YourBrand.Showroom.Application.Common.Models;

public record class Results<T>(IEnumerable<T> Items, int TotalCount);