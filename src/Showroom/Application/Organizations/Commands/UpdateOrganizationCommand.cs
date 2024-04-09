using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Organizations.Commands;

public record UpdateOrganizationCommand(string Id, string Name) : IRequest
{
    public class UpdateOrganizationCommandHandler(IShowroomContext context) : IRequestHandler<UpdateOrganizationCommand>
    {
        public async Task Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (organization is null) throw new Exception();

            organization.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}