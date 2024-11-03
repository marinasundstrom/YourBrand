using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record CreatePersonProfileCommand(CreatePersonProfileDto PersonProfile) : IRequest<PersonProfileDto>
{
    sealed class CreatePersonProfileCommandHandler(
        IShowroomContext context,
        IUrlHelper urlHelper) : IRequestHandler<CreatePersonProfileCommand, PersonProfileDto>
    {
        public async Task<PersonProfileDto> Handle(CreatePersonProfileCommand request, CancellationToken cancellationToken)
        {
            var personProfile = new PersonProfile
            {
                FirstName = request.PersonProfile.FirstName,
                LastName = request.PersonProfile.LastName,
                DisplayName = request.PersonProfile.DisplayName,
                Headline = request.PersonProfile.Headline,
                ShortPresentation = "",
                Presentation = "",
                IndustryId = request.PersonProfile.IndustryId
                //ManagerId = ""
            };

            if (personProfile.AvailableFromDate is not null)
            {
                personProfile.AvailableFromDate = personProfile.AvailableFromDate?.Date;
            }

            if (request.PersonProfile.OrganizationId is not null)
            {
                personProfile.OrganizationId = request.PersonProfile.OrganizationId;
            }

            if (request.PersonProfile.CompetenceAreaId is not null)
            {
                personProfile.CompetenceAreaId = request.PersonProfile.CompetenceAreaId;
            }

            context.PersonProfiles.Add(personProfile);

            await context.SaveChangesAsync(cancellationToken);

            personProfile = await context.PersonProfiles
                .Include(x => x.Industry)
                .Include(x => x.Organization)
                .Include(x => x.CompetenceArea)
                .FirstOrDefaultAsync(x => x.Id == personProfile.Id);

            return personProfile.ToDto(urlHelper);
        }
    }
}