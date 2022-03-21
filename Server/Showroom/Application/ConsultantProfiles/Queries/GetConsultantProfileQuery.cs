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

namespace YourCompany.Showroom.Application.ConsultantProfiles.Queries;

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
