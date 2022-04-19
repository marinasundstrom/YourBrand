using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Organizations
.Commands;

public record DeleteOrganizationCommand(string Id) : IRequest
{
    public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand>
    {
        private readonly ITimeReportContext context;

        public DeleteOrganizationCommandHandler(ITimeReportContext context)
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