using System.ComponentModel.DataAnnotations;

namespace Skynet.Showroom.Application.Cases;

public record CaseDto
(
    string Id,
    string? Description
);
