using System.ComponentModel.DataAnnotations;

using YourBrand.Marketing.Application;
using YourBrand.Marketing.Application.Common.Models;

namespace YourBrand.Marketing.Application.Discounts;

public record DiscountDto
(
    string Id,
    string Name
);
