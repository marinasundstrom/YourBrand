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

namespace YourBrand.Showroom.Application.Cases.Queries;

public record GetCaseQuery(string Id) : IRequest<CaseDto?>
{
    class GetCaseQueryHandler : IRequestHandler<GetCaseQuery, CaseDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IUrlHelper _urlHelper;

        public GetCaseQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<CaseDto?> Handle(GetCaseQuery request, CancellationToken cancellationToken)
        {
            var @case = await _context
               .Cases
               .FirstAsync(c => c.Id == request.Id);

            if (@case is null)
            {
                return null;
            }

            return @case.ToDto(_urlHelper);
        }
    }
}
