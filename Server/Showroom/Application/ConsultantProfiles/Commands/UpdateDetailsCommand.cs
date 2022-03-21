using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Domain.Entities;
using YourCompany.Showroom.Domain.Exceptions;

namespace YourCompany.Showroom.Application.ConsultantProfiles.Commands;

public record UpdateDetailsCommand(string Id, ConsultantProfileDetailsDto Details) : IRequest
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

        public async Task<Unit> Handle(UpdateDetailsCommand request, CancellationToken cancellationToken)
        {
            var consultantProfile = await _context.ConsultantProfiles.FindAsync(request.Id);
            if (consultantProfile is null)
            {
                throw new Exception();
            }

            consultantProfile.FirstName = request.Details.FirstName;
            consultantProfile.LastName = request.Details.LastName;
            consultantProfile.DisplayName = request.Details.DisplayName;
            consultantProfile.BirthDate = request.Details.BirthDate;
            consultantProfile.Location = request.Details.Location;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

public record ConsultantProfileDetailsDto(string FirstName, string LastName, string? DisplayName, DateTime? BirthDate, string? Location);