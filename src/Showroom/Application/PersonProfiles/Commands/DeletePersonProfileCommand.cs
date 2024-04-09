using MediatR;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record DeletePersonProfileCommand(string Id) : IRequest
{
    class DeletePersonProfileCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<DeletePersonProfileCommand>
    {
        public async Task Handle(DeletePersonProfileCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles.FindAsync(request.Id);
            if (personProfile == null)
            {
                throw new Exception("Not found");
            }

            context.PersonProfiles.Remove(personProfile);
            await context.SaveChangesAsync(cancellationToken);

        }
    }
}