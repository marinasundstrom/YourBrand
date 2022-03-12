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

namespace Skynet.Showroom.Application.ConsultantProfiles.Queries;

public class GetConsultantProfileQuery : IRequest<ConsultantProfileDto>
{
    public GetConsultantProfileQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }

    class GetConsultantProfileQueryHandler : IRequestHandler<GetConsultantProfileQuery, ConsultantProfileDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IUrlHelper _urlHelper;

        public GetConsultantProfileQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<ConsultantProfileDto?> Handle(GetConsultantProfileQuery request, CancellationToken cancellationToken)
        {
            var consultantProfile = await _context
               .ConsultantProfiles
               .Include(x => x.Organization)
               .Include(c => c.CompetenceArea)
               //.Include(c => c.Manager)
               .FirstAsync(c => c.Id == request.Id);

            if (consultantProfile is null)
            {
                return null;
            }

            return consultantProfile.ToDto(_urlHelper);
        }
    }
}
