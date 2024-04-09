using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.EventHandlers;

public class ExperienceHandler(IShowroomContext context) : IDomainEventHandler<ExperienceAdded>, IDomainEventHandler<ExperienceUpdated>, IDomainEventHandler<ExperienceRemoved>
{
    public async Task Handle(ExperienceAdded notification, CancellationToken cancellationToken)
    {
        await Update(notification.PersonProfileExperienceId, notification.PersonProfileId, notification.IndustryId, cancellationToken);
    }

    public async Task Handle(ExperienceUpdated notification, CancellationToken cancellationToken)
    {
        await Update(notification.PersonProfileExperienceId, notification.PersonProfileId, notification.IndustryId, cancellationToken);
    }

    public async Task Handle(ExperienceRemoved notification, CancellationToken cancellationToken)
    {
        await Update(notification.PersonProfileExperienceId, notification.PersonProfileId, notification.IndustryId, cancellationToken);
    }

    private async Task Update(string personProfileExperienceId, string personProfileId, int industryId, CancellationToken cancellationToken)
    {
        var experiences = context.PersonProfileExperiences
            .Where(x => x.PersonProfile.Id == personProfileId && x.Company.Industry.Id == industryId)
            .OrderBy(x => x.StartDate);

        if (!await experiences.AnyAsync(cancellationToken))
        {
            var pfie2 = await context.PersonProfileIndustryExperiences.FirstOrDefaultAsync(x => x.PersonProfile.Id == personProfileId && x.Industry.Id == industryId, cancellationToken);
            if (pfie2 is not null)
            {
                context.PersonProfileIndustryExperiences.Remove(pfie2);
            }

            return;
        }

        //var overlapping = AppointmentHelpers.Overlappings(experiences.Select(x => (x.StartDate , x.EndDate)));

        var yearsOfExperience = experiences.Sum(x => (x.EndDate ?? DateTime.Now).Year - x.StartDate.Year);

        var pfie = await context.PersonProfileIndustryExperiences.FirstOrDefaultAsync(x => x.PersonProfile.Id == personProfileId && x.Industry.Id == industryId, cancellationToken);
        if (pfie is null)
        {
            pfie = new Domain.Entities.PersonProfileIndustryExperiences()
            {
                Id = Guid.NewGuid().ToString(),
                PersonProfile = await context.PersonProfiles.FirstAsync(x => x.Id == personProfileId, cancellationToken)
            };
            context.PersonProfileIndustryExperiences.Add(pfie);
        }

        pfie.Industry = await context.Industries.FirstAsync(x => x.Id == industryId, cancellationToken);
        pfie.Years = yearsOfExperience;
    }
}