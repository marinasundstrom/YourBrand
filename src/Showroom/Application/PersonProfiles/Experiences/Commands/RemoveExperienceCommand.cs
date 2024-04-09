using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.Commands;

public record RemoveExperienceCommand(string PersonProfileId, string Id) : IRequest
{
    sealed class RemoveExperienceCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<RemoveExperienceCommand>
    {
        public async Task Handle(RemoveExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await context.PersonProfileExperiences
                .Include(x => x.PersonProfile)
                .Include(x => x.Company)
                .ThenInclude(x => x.Industry)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            if (experience is null)
            {
                throw new Exception("Not found");
            }

            context.PersonProfileExperiences.Remove(experience);

            experience.AddDomainEvent(new ExperienceUpdated(experience.PersonProfile.Id, experience.PersonProfile.Id, experience.Company.Industry.Id));

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}