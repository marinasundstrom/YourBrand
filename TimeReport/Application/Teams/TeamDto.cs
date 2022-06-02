using System.ComponentModel.DataAnnotations;

using YourBrand.TimeReport.Application;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Users;

namespace YourBrand.TimeReport.Application.Teams
;

public record TeamDto
(
    string Id,
    string Name,
    IEnumerable<TeamMemberDto> Members
);

public record TeamMemberDto(string Id, string FirstName, string LastName);

public record TeamMembershipDto(string Id, UserDto User);