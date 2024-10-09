using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Motions.Command;

public record EditMotionItem(string OrganizationId, int Id, string ItemId, string Text) : IRequest<Result<MotionItemDto>>
{
    public class Validator : AbstractValidator<EditMotionItem>
    {
        public Validator()
        {
            RuleFor(x => x.Text).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<EditMotionItem, Result<MotionItemDto>>
    {
        public async Task<Result<MotionItemDto>> Handle(EditMotionItem request, CancellationToken cancellationToken)
        {
            var motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            var motionItem = motion.Items.FirstOrDefault(x => x.Id == request.ItemId);

            if(motionItem is  null) 
            {
                return Errors.Motions.MotionItemNotFound;
            }
        
            motionItem.Text = request.Text;

            context.Motions.Update(motion);

            await context.SaveChangesAsync(cancellationToken);

            motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == motion.Id!, cancellationToken);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            return motionItem.ToDto();
        }
    }
}