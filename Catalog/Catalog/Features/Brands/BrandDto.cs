using System.ComponentModel.DataAnnotations;

namespace YourBrand.Catalog.Features.Brands;

public record BrandDto
(
    int Id,
    string Name,
    string Handle
);