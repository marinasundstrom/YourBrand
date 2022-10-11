using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.Commands;

public record UpdateExperienceCommand(
    string PersonProfileId,
    string Id,
    string Title,
    string CompanyId,
    string? Location,
    string EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : IRequest<ExperienceDto>
{
    class UpdateExperienceCommandHandler : IRequestHandler<UpdateExperienceCommand, ExperienceDto>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public UpdateExperienceCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ExperienceDto> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await _context.PersonProfileExperiences.FindAsync(request.Id);
            if (experience is null)
            {
                return null;
            }

            experience.Title = request.Title;
            experience.CompanyId = request.CompanyId;
            experience.EmploymentType = request.EmploymentType;
            experience.Location = request.Location;
            experience.StartDate = request.StartDate;
            experience.EndDate = request.EndDate;
            experience.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            experience = await _context.PersonProfileExperiences
                .Include(x => x.Employment)
                .ThenInclude(x => x.Employer)
                .Include(x => x.Company)
                .ThenInclude(x => x.Industry)
                .Include(x => x.Skills)
                .ThenInclude(x => x.PersonProfileSkill)
                .FirstAsync(x => x.Id == experience.Id);

            return experience.ToDto();
        }
    }
}
