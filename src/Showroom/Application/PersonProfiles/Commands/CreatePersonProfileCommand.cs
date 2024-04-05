using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record CreatePersonProfileCommand(CreatePersonProfileDto PersonProfile) : IRequest<PersonProfileDto>
{
    class CreatePersonProfileCommandHandler : IRequestHandler<CreatePersonProfileCommand, PersonProfileDto>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;
        private readonly IUrlHelper _urlHelper;

        public CreatePersonProfileCommandHandler(
            IShowroomContext context,
            IUserContext userContext,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.userContext = userContext;
            _urlHelper = urlHelper;
        }

        public async Task<PersonProfileDto> Handle(CreatePersonProfileCommand request, CancellationToken cancellationToken)
        {
            var personProfile = new PersonProfile
            {
                Id = Guid.NewGuid().ToString(),
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

            _context.PersonProfiles.Add(personProfile);

            await _context.SaveChangesAsync(cancellationToken);

            personProfile = await _context.PersonProfiles
                .Include(x => x.Industry)
                .Include(x => x.Organization)
                .Include(x => x.CompetenceArea)
                .FirstOrDefaultAsync(x => x.Id == personProfile.Id);

            return personProfile.ToDto(_urlHelper);
        }
    }
}