using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Queries;

public class ConsultantQuery
{
    public string? SearchString { get; set; }

    public string? OrganizationId { get; set; }

    public string? CompetenceAreaId { get; set; }

    public IEnumerable<QuerySkill> Skills { get; set; }
}

public class QuerySkill
{
    public string SkillId { get; set; }

    public SkillLevel Level{ get; set; }
}