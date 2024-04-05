using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Cases.Queries;

public record GetCaseQuery(string Id) : IRequest<CaseDto?>
{
    class GetCaseQueryHandler : IRequestHandler<GetCaseQuery, CaseDto?>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;
        private readonly IUrlHelper _urlHelper;

        public GetCaseQueryHandler(
            IShowroomContext context,
            IUserContext userContext,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.userContext = userContext;
            _urlHelper = urlHelper;
        }

        public async Task<CaseDto?> Handle(GetCaseQuery request, CancellationToken cancellationToken)
        {
            var @case = await _context.Cases
               .Include(c => c.CaseProfiles)
               .Include(c => c.CreatedBy)
               .Include(c => c.LastModifiedBy)
               .FirstAsync(c => c.Id == request.Id);

            if (@case is null)
            {
                return null;
            }

            return @case.ToDto(_urlHelper);
        }
    }
}