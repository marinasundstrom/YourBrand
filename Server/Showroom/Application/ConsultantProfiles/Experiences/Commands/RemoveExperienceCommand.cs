using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Domain.Entities;
using Skynet.Showroom.Domain.Exceptions;

namespace Skynet.Showroom.Application.ConsultantProfiles.Experiences.Commands;

public record RemoveExperienceCommand(string ConsultantProfileId, string Id) : IRequest
{
    class RemoveExperienceCommandHandler : IRequestHandler<RemoveExperienceCommand>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public RemoveExperienceCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(RemoveExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await _context.ConsultantProfileExperiences.FindAsync(request.Id, cancellationToken);
            if (experience == null)
            {
                throw new Exception("Not found");
            }

            _context.ConsultantProfileExperiences.Remove(experience);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
