using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Motions.Command;

public record RemoveMotionItem(string OrganizationId, int Id, string ParticipantId) : IRequest<Result<MotionDto>>
{
    public class Validator : AbstractValidator<RemoveMotionItem>
    {
        public Validator()
        {

        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<RemoveMotionItem, Result<MotionDto>>
    {
        public async Task<Result<MotionDto>> Handle(RemoveMotionItem request, CancellationToken cancellationToken)
        {
            var motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            var motionItem = motion.Items.FirstOrDefault(x => x.Id == request.ParticipantId);

            if (motionItem is null)
            {
                return Errors.Motions.MotionItemNotFound;
            }

            motion.RemoveItem(motionItem);

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