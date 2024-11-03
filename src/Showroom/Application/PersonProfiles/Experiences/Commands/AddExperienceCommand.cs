﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Events.Enums;

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
    sealed class CreatePersonProfileCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<AddExperienceCommand, ExperienceDto>
    {
        public async Task<ExperienceDto> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles.FirstAsync(cp => cp.Id == request.PersonProfileId, cancellationToken);

            var company = await context.Companies
                .Include(x => x.Industry)
                .FirstAsync(cp => cp.Id == request.CompanyId, cancellationToken);

            var experience = new PersonProfileExperience
            {
                PersonProfile = personProfile,
                Title = request.Title,
                CompanyId = request.CompanyId,
                EmploymentType = request.EmploymentType,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description
            };

            context.PersonProfileExperiences.Add(experience);

            experience.AddDomainEvent(new ExperienceUpdated(experience.PersonProfile.Id, personProfile.Id, company.Industry.Id));

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