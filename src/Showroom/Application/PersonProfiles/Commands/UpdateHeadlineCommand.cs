using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdateHeadlineCommand(string Id, string Text) : IRequest
{
    class UpdateHeadlineCommandHandler : IRequestHandler<UpdateHeadlineCommand>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public UpdateHeadlineCommandHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task Handle(UpdateHeadlineCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await _context.PersonProfiles.FindAsync(request.Id);
            if (personProfile is null)
            {
                throw new Exception();
            }

            personProfile.Headline = request.Text;

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}