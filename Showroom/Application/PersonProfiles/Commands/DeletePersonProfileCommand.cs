using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record DeletePersonProfileCommand(string Id) : IRequest
{
    class DeletePersonProfileCommandHandler : IRequestHandler<DeletePersonProfileCommand>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public DeletePersonProfileCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task Handle(DeletePersonProfileCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await _context.PersonProfiles.FindAsync(request.Id);
            if (personProfile == null)
            {
                throw new Exception("Not found");
            }

            _context.PersonProfiles.Remove(personProfile);
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
