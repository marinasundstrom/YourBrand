using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UpdatePresentationCommand(string Id, string Text) : IRequest
{
    class UpdatePresentationCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<UpdatePresentationCommand>
    {
        public async Task Handle(UpdatePresentationCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles.FindAsync(request.Id);
            if (personProfile is null)
            {
                throw new Exception();
            }

            personProfile.Presentation = request.Text;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}