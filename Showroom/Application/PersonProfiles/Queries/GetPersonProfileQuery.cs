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

namespace YourBrand.Showroom.Application.PersonProfiles.Queries;

public record GetPersonProfileQuery(string Id) : IRequest<PersonProfileDto>
{
    class GetPersonProfileQueryHandler : IRequestHandler<GetPersonProfileQuery, PersonProfileDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IUrlHelper _urlHelper;

        public GetPersonProfileQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<PersonProfileDto?> Handle(GetPersonProfileQuery request, CancellationToken cancellationToken)
        {
            var personProfile = await _context
               .PersonProfiles
               .Include(x => x.Industry)
               .Include(x => x.Organization)
               .Include(c => c.CompetenceArea)
               //.Include(c => c.Manager)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (personProfile is null)
            {
                return null;
            }

            return personProfile.ToDto(_urlHelper);
        }
    }
}
