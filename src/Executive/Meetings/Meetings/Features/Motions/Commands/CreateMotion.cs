using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Motions.Command;

public sealed record CreateMotionItemDto(string Text);

public record CreateMotion(string OrganizationId, string Title, string Text, IEnumerable<CreateMotionItemDto> Items) : IRequest<Result<MotionDto>>
{
    public class Validator : AbstractValidator<CreateMotion>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            //RuleFor(x => x.Items).NotEmpty();
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<CreateMotion, Result<MotionDto>>
    {
        public async Task<Result<MotionDto>> Handle(CreateMotion request, CancellationToken cancellationToken)
        {
            int id = 1;

            try
            {
                id = await context.Motions
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var motion = new Motion(id, request.Title);
            motion.OrganizationId = request.OrganizationId;
            motion.Text = request.Text;

            foreach (var motionItem in request.Items) 
            {
                motion.AddItem(motionItem.Text);
            }

            context.Motions.Add(motion);

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