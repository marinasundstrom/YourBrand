using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record DeletePersonProfileCommand(string Id) : IRequest
{
    class DeletePersonProfileCommandHandler : IRequestHandler<DeletePersonProfileCommand>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public DeletePersonProfileCommandHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
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