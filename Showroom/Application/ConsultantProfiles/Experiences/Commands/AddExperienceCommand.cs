using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Experiences.Commands;

public record AddExperienceCommand(
    string ConsultantProfileId,
    string Title,
    string CompanyName,
    string? Location,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : IRequest<ExperienceDto>
{
    class CreateConsultantProfileCommandHandler : IRequestHandler<AddExperienceCommand, ExperienceDto>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public CreateConsultantProfileCommandHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ExperienceDto> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
        {
            var consultantProfile = await _context.ConsultantProfiles.FirstAsync(cp => cp.Id == request.ConsultantProfileId, cancellationToken);

            var experience = new ConsultantProfileExperience
            {
                Id = Guid.NewGuid().ToString(),
                ConsultantProfile = consultantProfile,
                Title = request.Title,
                CompanyName = request.CompanyName,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description
            };

            _context.ConsultantProfileExperiences.Add(experience);

            await _context.SaveChangesAsync(cancellationToken);

            experience = await _context.ConsultantProfileExperiences
                .FirstAsync(x => x.Id == experience.Id);

            return experience.ToDto();
        }
    }
}
