using YourBrand.Showroom.Application.Industries;

namespace YourBrand.Showroom.Application.Companies;

public record CompanyDto(
    string Id,
    string Name,
    string? Logo,
    string? Link,
    IndustryDto Industry
);