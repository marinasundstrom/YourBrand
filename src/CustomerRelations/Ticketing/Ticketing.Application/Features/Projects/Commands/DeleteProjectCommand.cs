
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Commands;

public record DeleteProjectCommand(string OrganizationId, int ProjectId) : IRequest<Result>
{
    public class DeleteProjectCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProjectCommand, Result>
    {
        public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return Errors.Projects.ProjectNotFound;
            }

            context.Projects.Remove(project);

            await context.SaveChangesAsync(cancellationToken);       

            return Result.Success;
        }
    }
}