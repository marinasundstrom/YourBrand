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

namespace Skynet.Showroom.Application.ConsultantProfiles.Experiences.Queries;

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
               .FirstAsync(c => c.Id == request.Id);

            if (experience is null)
            {
                return null;
            }

            return experience.ToDto();
        }
    }
}
