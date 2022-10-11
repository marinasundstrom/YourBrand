using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Search.Queries;

public class PersonProfileQuery
{
    public string? SearchString { get; set; }

    public int? IndustryId { get; set; }

    public int? TitleId { get; set; }

    public string? OrganizationId { get; set; }

    public string? CompetenceAreaId { get; set; }

    public IEnumerable<QuerySkill> Skills { get; set; }

    public IEnumerable<QueryExperience> Experiences { get; set; }
}

public class QuerySkill
{
    public string SkillId { get; set; }

    public SkillLevel Level{ get; set; }
}

public class QueryExperience
{
    public ConditionalOperator Condition { get; set; }

    public int IndustryId { get; set; }

    public int Years { get; set; }
}

public enum ConditionalOperator
{
    And = 1,
    Or = 2
}