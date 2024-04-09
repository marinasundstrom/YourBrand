using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Cases.Queries;

public record GetCaseQuery(string Id) : IRequest<CaseDto?>
{
    sealed class GetCaseQueryHandler(
        IShowroomContext context,
        IUrlHelper urlHelper) : IRequestHandler<GetCaseQuery, CaseDto?>
    {
        public async Task<CaseDto?> Handle(GetCaseQuery request, CancellationToken cancellationToken)
        {
            var @case = await context.Cases
               .Include(c => c.CaseProfiles)
               .Include(c => c.CreatedBy)
               .Include(c => c.LastModifiedBy)
               .FirstAsync(c => c.Id == request.Id);

            if (@case is null)
            {
                return null;
            }

            return @case.ToDto(urlHelper);
        }
    }
}