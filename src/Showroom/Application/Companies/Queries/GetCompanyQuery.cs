using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.Companies.Queries;

public record GetCompanyQuery(string Id) : IRequest<CompanyDto?>
{
    class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, CompanyDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetCompanyQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
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
