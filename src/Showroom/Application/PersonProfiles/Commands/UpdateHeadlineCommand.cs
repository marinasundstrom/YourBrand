using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdateHeadlineCommand(string Id, string Text) : IRequest
{
    class UpdateHeadlineCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<UpdateHeadlineCommand>
    {
        public async Task Handle(UpdateHeadlineCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles.FindAsync(request.Id);
            if (personProfile is null)
            {
                throw new Exception();
            }

            personProfile.Headline = request.Text;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}