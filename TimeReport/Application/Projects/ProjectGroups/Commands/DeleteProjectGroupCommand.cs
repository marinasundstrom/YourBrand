
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.ProjectGroups.Commands;

public record DeleteProjectGroupCommand(string ExpenseId) : IRequest
{
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteProjectGroupCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProjectGroupCommand request, CancellationToken cancellationToken)
        {
            var projectGroup = await _context.ProjectGroups
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (projectGroup is null)
            {
                throw new Exception();
            }

            _context.ProjectGroups.Remove(projectGroup);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}