using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Domain.Entities;
using YourCompany.Showroom.Domain.Exceptions;

namespace YourCompany.Showroom.Application.Cases.Queries;

public class GetCaseQuery : IRequest<CaseDto?>
{
    public GetCaseQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }

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
