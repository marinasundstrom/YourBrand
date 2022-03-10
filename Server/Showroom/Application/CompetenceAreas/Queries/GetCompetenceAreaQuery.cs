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

namespace Skynet.Showroom.Application.CompetenceAreas.Queries;

public class GetCompetenceAreaQuery : IRequest<CompetenceAreaDto?>
{
    public GetCompetenceAreaQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }

    class GetCompetenceAreaQueryHandler : IRequestHandler<GetCompetenceAreaQuery, CompetenceAreaDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetCompetenceAreaQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<CompetenceAreaDto?> Handle(GetCompetenceAreaQuery request, CancellationToken cancellationToken)
        {
            var competenceArea = await _context
               .CompetenceAreas
               .FirstAsync(c => c.Id == request.Id);

            if (competenceArea is null)
            {
                return null;
            }

            return competenceArea.ToDto();
        }
    }
}
