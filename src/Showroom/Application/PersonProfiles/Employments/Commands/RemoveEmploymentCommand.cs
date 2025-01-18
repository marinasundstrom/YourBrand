using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Employments.Commands;

public record RemoveEmploymentCommand(string PersonProfileId, string Id) : IRequest
{
    sealed class RemoveEmploymentCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<RemoveEmploymentCommand>
    {
        public async Task Handle(RemoveEmploymentCommand request, CancellationToken cancellationToken)
        {
            var employment = await context.Employments
                .Include(x => x.PersonProfile)
                .Include(x => x.Employer)
                .ThenInclude(x => x.Industry)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            if (employment is null)
            {
                throw new Exception("Not found");
            }

            context.Employments.Remove(employment);

            //employment.AddDomainEvent(new EmploymentUpdated(employment.PersonProfile.Id, employment.PersonProfile.Id, employment.Company.Industry.Id));

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}