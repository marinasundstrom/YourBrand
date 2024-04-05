using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Companies.Queries;

public record GetCompanyQuery(string Id) : IRequest<CompanyDto?>
{
    class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, CompanyDto?>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public GetCompanyQueryHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<CompanyDto?> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await _context
               .Companies
               .Include(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (company is null)
            {
                return null;
            }

            return company.ToDto();
        }
    }
}