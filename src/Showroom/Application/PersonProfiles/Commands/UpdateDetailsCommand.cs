using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdateDetailsCommand(string Id, PersonProfileDetailsDto Details) : IRequest
{
    class UpdateDetailsCommandHandler : IRequestHandler<UpdateDetailsCommand>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public UpdateDetailsCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task Handle(UpdateDetailsCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await _context.PersonProfiles.FindAsync(request.Id);
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
            personProfile.OrganizationId= request.Details.OrganizationId;
            personProfile.CompetenceAreaId = request.Details.CompetenceAreaId;

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}

public record PersonProfileDetailsDto(string FirstName, string LastName, string? DisplayName, DateTime? BirthDate, string? Location, int IndustryId, string OrganizationId, string CompetenceAreaId);