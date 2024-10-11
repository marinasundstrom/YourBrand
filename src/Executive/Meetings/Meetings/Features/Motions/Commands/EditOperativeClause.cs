using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Motions.Command;

public record EditOperativeClause(string OrganizationId, int Id, string ItemId, OperativeAction Action, string Text) : IRequest<Result<MotionOperativeClauseDto>>
{
    public class Validator : AbstractValidator<EditOperativeClause>
    {
        public Validator()
        {
            RuleFor(x => x.Text).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<EditOperativeClause, Result<MotionOperativeClauseDto>>
    {
        public async Task<Result<MotionOperativeClauseDto>> Handle(EditOperativeClause request, CancellationToken cancellationToken)
        {
            var motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            var operativeClause = motion.OperativeClauses.FirstOrDefault(x => x.Id == request.ItemId);

            if(operativeClause is  null) 
            {
                return Errors.Motions.OperativeClauseNotFound;
            }
        
            operativeClause.Text = request.Text;

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