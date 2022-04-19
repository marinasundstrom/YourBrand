using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Commands;

public record DeleteConsultantProfileCommand(string Id) : IRequest
{
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
