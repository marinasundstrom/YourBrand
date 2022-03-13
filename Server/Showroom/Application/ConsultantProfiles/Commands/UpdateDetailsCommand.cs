using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Domain.Entities;
using Skynet.Showroom.Domain.Exceptions;

namespace Skynet.Showroom.Application.ConsultantProfiles.Commands;

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

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

public record ConsultantProfileDetailsDto(string FirstName, string LastName, string? DisplayName);