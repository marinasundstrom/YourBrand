using System.ComponentModel.DataAnnotations;

using YourCompany.Showroom.Application;
using YourCompany.Showroom.Application.Common.Models;

namespace YourCompany.Showroom.Application.Organizations;

public record OrganizationDto
(
    string Id,
    string Name,
    AddressDto Address
);
