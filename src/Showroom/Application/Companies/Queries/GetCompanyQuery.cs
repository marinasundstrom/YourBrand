using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Companies.Queries;

public record GetCompanyQuery(string Id) : IRequest<CompanyDto?>
{
    class GetCompanyQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetCompanyQuery, CompanyDto?>
    {
        public async Task<CompanyDto?> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await context
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