using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdateHeadlineCommand(string Id, string Text) : IRequest
{
    class UpdateHeadlineCommandHandler : IRequestHandler<UpdateHeadlineCommand>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public UpdateHeadlineCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
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