using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Queries;

public record GetConsultantProfileQuery(string Id) : IRequest<ConsultantProfileDto>
{
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
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (consultantProfile is null)
            {
                return null;
            }

            return consultantProfile.ToDto(_urlHelper);
        }
    }
}
