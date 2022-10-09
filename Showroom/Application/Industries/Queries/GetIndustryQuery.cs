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

namespace YourBrand.Showroom.Application.Industries.Queries;

public record GetIndustryQuery(int Id) : IRequest<IndustryDto?>
{
    class GetIndustryQueryHandler : IRequestHandler<GetIndustryQuery, IndustryDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetIndustryQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<IndustryDto?> Handle(GetIndustryQuery request, CancellationToken cancellationToken)
        {
            var industry = await _context
               .Industries
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (industry is null)
            {
                return null;
            }

            return industry.ToDto();
        }
    }
}
