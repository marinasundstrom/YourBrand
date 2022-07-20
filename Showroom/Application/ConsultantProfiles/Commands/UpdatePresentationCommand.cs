using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Commands;

public record UpdatePresentationCommand(string Id, string Text) : IRequest
{
    class UpdatePresentationCommandHandler : IRequestHandler<UpdatePresentationCommand>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public UpdatePresentationCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(UpdatePresentationCommand request, CancellationToken cancellationToken)
        {
            var consultantProfile = await _context.ConsultantProfiles.FindAsync(request.Id);
            if (consultantProfile is null)
            {
                throw new Exception();
            }

            consultantProfile.Presentation = request.Text;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
