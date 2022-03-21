using System;

namespace YourCompany.Showroom.Application.Common.Models;

public record class Results<T>(IEnumerable<T> Items, int TotalCount);