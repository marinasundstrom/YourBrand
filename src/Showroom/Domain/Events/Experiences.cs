using YourBrand.Domain;
using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Events.Enums;

public record ExperienceAdded : DomainEvent
{
    public ExperienceAdded(string personProfileExperienceId, string personProfileId, int industryId)
    {
        PersonProfileExperienceId = personProfileExperienceId;
        PersonProfileId = personProfileId;
        IndustryId = industryId;
    }

    public string PersonProfileExperienceId { get; }
    public string PersonProfileId { get; }
    public int IndustryId { get; }
}

public record ExperienceRemoved : DomainEvent
{
    public ExperienceRemoved(string personProfileExperienceId, string personProfileId, int industryId)
    {
        PersonProfileExperienceId = personProfileExperienceId;
        PersonProfileId = personProfileId;
        IndustryId = industryId;
    }

    public string PersonProfileExperienceId { get; }
    public string PersonProfileId { get; }
    public int IndustryId { get; }
}

public record ExperienceUpdated : DomainEvent
{
    public ExperienceUpdated(string personProfileExperienceId, string personProfileId, int industryId)
    {
        PersonProfileExperienceId = personProfileExperienceId;
        PersonProfileId = personProfileId;
        IndustryId = industryId;
    }

    public string PersonProfileExperienceId { get; }
    public string PersonProfileId { get; }
    public int IndustryId { get; }
}