using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.EventHandlers;

public class ExperienceHandler : IDomainEventHandler<ExperienceAdded>, IDomainEventHandler<ExperienceUpdated>, IDomainEventHandler<ExperienceRemoved>
{
    private readonly IShowroomContext _context;

    public ExperienceHandler(IShowroomContext context) 
    {
        _context = context;
    }

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
        var experiences = _context.PersonProfileExperiences
            .Where(x => x.PersonProfile.Id == personProfileId && x.Company.Industry.Id == industryId)
            .OrderBy(x => x.StartDate);


        if(!await experiences.AnyAsync(cancellationToken)) 
        {
            var pfie2 = await _context.PersonProfileIndustryExperiences.FirstOrDefaultAsync(x => x.PersonProfile.Id == personProfileId && x.Industry.Id == industryId, cancellationToken); 
            if(pfie2 is not null) 
            {
                _context.PersonProfileIndustryExperiences.Remove(pfie2);
            }

            return;
        }

        //var overlapping = AppointmentHelpers.Overlappings(experiences.Select(x => (x.StartDate , x.EndDate)));

        var yearsOfExperience = experiences.Sum(x => (x.EndDate ?? DateTime.Now).Year - x.StartDate.Year);

        var pfie = await _context.PersonProfileIndustryExperiences.FirstOrDefaultAsync(x => x.PersonProfile.Id == personProfileId && x.Industry.Id == industryId, cancellationToken); 
        if(pfie is null) 
        {
            pfie = new Domain.Entities.PersonProfileIndustryExperiences() {
                Id = Guid.NewGuid().ToString(),
                PersonProfile  = await _context.PersonProfiles.FirstAsync(x => x.Id == personProfileId, cancellationToken)
            };
            _context.PersonProfileIndustryExperiences.Add(pfie);
        }

        pfie.Industry = await _context.Industries.FirstAsync(x => x.Id == industryId, cancellationToken);
        pfie.Years = yearsOfExperience;
    }
}