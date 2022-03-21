using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Domain.Entities;
using YourCompany.Showroom.Domain.Exceptions;

namespace YourCompany.Showroom.Application.ConsultantProfiles.Experiences.Commands;

public record UpdateExperienceCommand(
    string ConsultantProfileId,
    string Id,
    string Title,
    string CompanyName,
    string? Location,
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
            var experience = await _context.ConsultantProfileExperiences.FindAsync(request.Id);
            if (experience is null)
            {
                return null;
            }

            experience.Title = request.Title;
            experience.CompanyName = request.CompanyName;
            experience.Location = request.Location;
            experience.StartDate = request.StartDate;
            experience.EndDate = request.EndDate;
            experience.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return experience.ToDto();
        }
    }
}
