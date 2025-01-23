using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.Commands;

public record UpdateExperienceCommand(
    string PersonProfileId,
    string Id,
    ExperienceType Type,
    string Title,
    string CompanyId,
    string? Location,
    string EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : IRequest<ExperienceDto>
{
    sealed class UpdateExperienceCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<UpdateExperienceCommand, ExperienceDto>
    {
        public async Task<ExperienceDto> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await context.PersonProfileExperiences
                .Include(x => x.PersonProfile)
                .Include(x => x.Company)
                .ThenInclude(x => x.Industry)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            if (experience is null)
            {
                throw new Exception("Not found");
            }

            experience.Title = request.Title;
            experience.CompanyId = request.CompanyId;
            experience.EmploymentType = request.EmploymentType;
            experience.Location = request.Location;
            experience.StartDate = request.StartDate;
            experience.EndDate = request.EndDate;
            experience.Description = request.Description;

            experience.AddDomainEvent(new ExperienceUpdated(experience.PersonProfile.Id, experience.PersonProfile.Id, experience.Company.Industry.Id));

            await context.SaveChangesAsync(cancellationToken);

            experience = await context.PersonProfileExperiences
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