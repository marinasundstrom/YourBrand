using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Domain.Entities;
using Skynet.Showroom.Domain.Exceptions;

namespace Skynet.Showroom.Application.Cases.Queries;

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

        public GetCaseQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
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

            return @case.ToDto();
        }
    }
}
