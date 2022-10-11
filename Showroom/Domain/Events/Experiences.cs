using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Events.Enums;

public class ExperienceAdded : DomainEvent
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

public class ExperienceRemoved : DomainEvent
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

public class ExperienceUpdated : DomainEvent
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