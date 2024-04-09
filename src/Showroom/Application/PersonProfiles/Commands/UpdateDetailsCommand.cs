using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdateDetailsCommand(string Id, PersonProfileDetailsDto Details) : IRequest
{
    class UpdateDetailsCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<UpdateDetailsCommand>
    {
        public async Task Handle(UpdateDetailsCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles.FindAsync(request.Id);
            if (personProfile is null)
            {
                throw new Exception();
            }

            personProfile.FirstName = request.Details.FirstName;
            personProfile.LastName = request.Details.LastName;
            personProfile.DisplayName = request.Details.DisplayName;
            personProfile.BirthDate = request.Details.BirthDate;
            personProfile.Location = request.Details.Location;
            personProfile.IndustryId = request.Details.IndustryId;
            personProfile.OrganizationId = request.Details.OrganizationId;
            personProfile.CompetenceAreaId = request.Details.CompetenceAreaId;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}

public record PersonProfileDetailsDto(string FirstName, string LastName, string? DisplayName, DateTime? BirthDate, string? Location, int IndustryId, string OrganizationId, string CompetenceAreaId);