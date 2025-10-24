using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Minutes;

namespace YourBrand.Meetings.Features.Minutes.Tasks;

public sealed record CompleteMinutesTask(string OrganizationId, string TaskId) : IRequest<Result>;

public sealed class CompleteMinutesTaskHandler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<CompleteMinutesTask, Result>
{
    public async Task<Result> Handle(CompleteMinutesTask request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null || string.IsNullOrWhiteSpace(userContext.UserId.Value))
        {
            return Errors.Users.UserNotFound;
        }

        var currentUserId = userContext.UserId.Value;

        var task = await context.MinutesTasks
            .InOrganization(request.OrganizationId)
            .Include(x => x.Minutes)
                .ThenInclude(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

        if (task is null)
        {
            return Errors.Minutes.MinutesTaskNotFound;
        }

        if (task.AssignedToId != currentUserId)
        {
            return Errors.Minutes.YouAreNotAssignedToThisTask;
        }

        task.Complete(currentUserId, DateTimeOffset.UtcNow);

        task.Minutes?.UpdateStateFromTasks();

        context.MinutesTasks.Update(task);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
