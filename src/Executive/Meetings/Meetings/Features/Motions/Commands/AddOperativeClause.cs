using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Motions.Command;

public record AddOperativeClause(string OrganizationId, int Id, OperativeAction Action, string Text) : IRequest<Result<MotionOperativeClauseDto>>
{
    public class Validator : AbstractValidator<AddOperativeClause>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<AddOperativeClause, Result<MotionOperativeClauseDto>>
    {
        public async Task<Result<MotionOperativeClauseDto>> Handle(AddOperativeClause request, CancellationToken cancellationToken)
        {
            var motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            var operativeClause = motion.AddOperativeClause(request.Action, request.Text);

            context.Motions.Update(motion);

            await context.SaveChangesAsync(cancellationToken);

            motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == motion.Id!, cancellationToken);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            return operativeClause.ToDto();
        }
    }
}