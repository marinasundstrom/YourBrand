using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdatePersonProfileCommand(string Id, UpdatePersonProfileDto PersonProfile) : IRequest<PersonProfileDto>
{
    class UpdatePersonProfileCommandHandler(
        IShowroomContext context,
        IUrlHelper urlHelper) : IRequestHandler<UpdatePersonProfileCommand, PersonProfileDto>
    {
        public async Task<PersonProfileDto> Handle(UpdatePersonProfileCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles.FindAsync(request.Id);
            if (personProfile is null)
            {
                return null;
            }

            personProfile.FirstName = request.PersonProfile.FirstName;
            personProfile.LastName = request.PersonProfile.LastName;
            personProfile.DisplayName = request.PersonProfile.DisplayName;
            personProfile.OrganizationId = request.PersonProfile.OrganizationId;
            personProfile.CompetenceAreaId = request.PersonProfile.CompetenceAreaId;
            personProfile.ManagerId = "";
            personProfile.ShortPresentation = "";
            personProfile.Presentation = "";

            if (personProfile.AvailableFromDate is not null)
            {
                personProfile.AvailableFromDate = personProfile.AvailableFromDate?.Date;
            }

            await context.SaveChangesAsync(cancellationToken);

            return personProfile.ToDto(urlHelper);
        }
    }
}