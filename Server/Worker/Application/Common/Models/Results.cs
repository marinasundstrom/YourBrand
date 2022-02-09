using System;

namespace Worker.Application.Common.Models;

public record class Results<T>(IEnumerable<T> Items, int TotalCount);