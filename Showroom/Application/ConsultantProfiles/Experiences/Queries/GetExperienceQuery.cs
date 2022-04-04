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

namespace YourBrand.Showroom.Application.ConsultantProfiles.Experiences.Queries;

public record GetExperienceQuery(string ConsultantProfileId, string Id) : IRequest<ExperienceDto>
{
    class GetConsultantProfileQueryHandler : IRequestHandler<GetExperienceQuery, ExperienceDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetConsultantProfileQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ExperienceDto?> Handle(GetExperienceQuery request, CancellationToken cancellationToken)
        {
            var experience = await _context
               .ConsultantProfileExperiences
               //.Include(c => c.Manager)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (experience is null)
            {
                return null;
            }

            return experience.ToDto();
        }
    }
}
