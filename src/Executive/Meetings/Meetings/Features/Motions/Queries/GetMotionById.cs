using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Motions.Queries;

public record GetMotionById(string OrganizationId, int Id) : IRequest<Result<MotionDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetMotionById, Result<MotionDto>>
    {
        public async Task<Result<MotionDto>> Handle(GetMotionById request, CancellationToken cancellationToken)
        {
            var motion = await context.Motions
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .Include(x => x.OperativeClauses.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (motion is null)
            {
                return Errors.Motions.MotionNotFound;
            }

            return motion.ToDto();
        }
    }
}