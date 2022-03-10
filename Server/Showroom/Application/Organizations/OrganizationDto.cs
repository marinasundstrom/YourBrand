using System.ComponentModel.DataAnnotations;

using Skynet.Showroom.Application;
using Skynet.Showroom.Application.Common.Models;

namespace Skynet.Showroom.Application.Organizations;

public record OrganizationDto
(
    string Id,
    string Name,
    AddressDto Address
);
