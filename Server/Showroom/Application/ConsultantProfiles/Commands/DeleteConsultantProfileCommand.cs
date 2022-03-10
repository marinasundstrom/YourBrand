using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Domain.Entities;
using Skynet.Showroom.Domain.Exceptions;

namespace Skynet.Showroom.Application.ConsultantProfiles.Commands
{
    public class DeleteConsultantProfileCommand : IRequest
    {
        public DeleteConsultantProfileCommand(string id)
        {
            Id = id;
        }

        public string Id { get; }

        class DeleteConsultantProfileCommandHandler : IRequestHandler<DeleteConsultantProfileCommand>
        {
            private readonly IShowroomContext _context;
            private readonly ICurrentUserService currentUserService;

            public DeleteConsultantProfileCommandHandler(
                IShowroomContext context,
                ICurrentUserService currentUserService)
            {
                _context = context;
                this.currentUserService = currentUserService;
            }

            public async Task<Unit> Handle(DeleteConsultantProfileCommand request, CancellationToken cancellationToken)
            {
                var consultantProfile = await _context.ConsultantProfiles.FindAsync(request.Id);
                if (consultantProfile == null)
                {
                    throw new Exception("Not found");
                }

                _context.ConsultantProfiles.Remove(consultantProfile);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
