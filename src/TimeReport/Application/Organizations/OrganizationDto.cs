using System.ComponentModel.DataAnnotations;

using YourBrand.TimeReport.Application;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Organizations
;

public record OrganizationDto
(
    string Id,
    string Name
);
