
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.ProjectGroups.Commands;

public record DeleteProjectGroupCommand(string OrganizationId, string ExpenseId) : IRequest
{
    public class DeleteExpenseCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProjectGroupCommand>
    {
        public async Task Handle(DeleteProjectGroupCommand request, CancellationToken cancellationToken)
        {
            var projectGroup = await context.ProjectGroups
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (projectGroup is null)
            {
                throw new Exception();
            }

            context.ProjectGroups.Remove(projectGroup);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}