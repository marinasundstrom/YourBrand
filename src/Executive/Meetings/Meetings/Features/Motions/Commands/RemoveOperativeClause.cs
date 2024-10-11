using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Motions.Command;

public record RemoveOperativeClause(string OrganizationId, int Id, string OperativeClauseId) : IRequest<Result<MotionDto>>
{
    public class Validator : AbstractValidator<RemoveOperativeClause>
    {
        public Validator()
        {

        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<RemoveOperativeClause, Result<MotionDto>>
    {
        public async Task<Result<MotionDto>> Handle(RemoveOperativeClause request, CancellationToken cancellationToken)
        {
            var motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            var operativeClause = motion.OperativeClauses.FirstOrDefault(x => x.Id == request.OperativeClauseId);

            if (operativeClause is null)
            {
                return Errors.Motions.OperativeClauseNotFound;
            }

            motion.RemoveOperativeClause(operativeClause);

            context.Motions.Update(motion);

            await context.SaveChangesAsync(cancellationToken);

            motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == motion.Id!, cancellationToken);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            return motion.ToDto();
        }
    }
}