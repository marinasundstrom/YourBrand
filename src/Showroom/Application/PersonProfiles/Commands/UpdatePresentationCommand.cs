using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdatePresentationCommand(string Id, string Text) : IRequest
{
    class UpdatePresentationCommandHandler : IRequestHandler<UpdatePresentationCommand>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public UpdatePresentationCommandHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task Handle(UpdatePresentationCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await _context.PersonProfiles.FindAsync(request.Id);
            if (personProfile is null)
            {
                throw new Exception();
            }

            personProfile.Presentation = request.Text;

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}