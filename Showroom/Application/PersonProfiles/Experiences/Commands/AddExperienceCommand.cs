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

public record AddExperienceCommand(
    string PersonProfileId,
    string Title,
    string CompanyId,
    string? Location,
    string EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : IRequest<ExperienceDto>
{
    class CreatePersonProfileCommandHandler : IRequestHandler<AddExperienceCommand, ExperienceDto>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public CreatePersonProfileCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ExperienceDto> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await _context.PersonProfiles.FirstAsync(cp => cp.Id == request.PersonProfileId, cancellationToken);

            var experience = new PersonProfileExperience
            {
                Id = Guid.NewGuid().ToString(),
                PersonProfile = personProfile,
                Title = request.Title,
                CompanyId = request.CompanyId,
                EmploymentType = request.EmploymentType,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description
            };

            _context.PersonProfileExperiences.Add(experience);

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
