using MediatR;

using Microsoft.EntityFrameworkCore;
using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Application.Organizations.Commands;

public record DeleteOrganizationCommand(string Id) : IRequest
{
    public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand>
    {
        private readonly IShowroomContext context;

        public DeleteOrganizationCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (organization is null) throw new Exception();

            context.Organizations.Remove(organization);
           
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}