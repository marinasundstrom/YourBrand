
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.ProjectGroups.Commands;

public record DeleteProjectGroupCommand(string OrganizationId, string ExpenseId) : IRequest
{
    public class DeleteExpenseCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteProjectGroupCommand>
    {
        public async Task Handle(DeleteProjectGroupCommand request, CancellationToken cancellationToken)
        {
            var projectGroup = await context.ProjectGroups
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