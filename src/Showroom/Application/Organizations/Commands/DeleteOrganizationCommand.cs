using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Organizations.Commands;

public record DeleteOrganizationCommand(string Id) : IRequest
{
    public class DeleteOrganizationCommandHandler(IShowroomContext context) : IRequestHandler<DeleteOrganizationCommand>
    {
        public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (organization is null) throw new Exception();

            context.Organizations.Remove(organization);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}