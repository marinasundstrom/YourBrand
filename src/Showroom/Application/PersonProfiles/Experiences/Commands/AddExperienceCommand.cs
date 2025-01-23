using System.Text.Json.Serialization;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.Commands;

public record AddExperienceCommand(
    string PersonProfileId,
    ExperienceDetailsDto ExperienceDetails)
    : IRequest<ExperienceDto>
{
    sealed class Handler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<AddExperienceCommand, ExperienceDto>
    {
        public async Task<ExperienceDto> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles
                .FirstAsync(cp => cp.Id == request.PersonProfileId, cancellationToken);

            var details = request.ExperienceDetails;

            if(details is EmploymentDetailsDto employmentDetails) 
            {
                var company = await context.Companies
                    .Include(x => x.Industry)
                    .FirstAsync(cp => cp.Id == employmentDetails.CompanyId, cancellationToken);

                var employment = new Employment
                {
                    Employer = company,
                    EmploymentType = employmentDetails.EmploymentType,
                    Location = employmentDetails.Location,
                    StartDate = employmentDetails.StartDate,
                    EndDate = employmentDetails.EndDate,
                };

                employment.Roles.Add(new EmploymentRole 
                {
                    PersonProfile = personProfile,
                    Title = employmentDetails.Title,
                    Description = employmentDetails.Description,
                    StartDate = employmentDetails.StartDate,
                    EndDate = employmentDetails.EndDate,
                });

                personProfile.Employments.Add(employment);

                await context.SaveChangesAsync(cancellationToken);

                employment = await context.Employments
                    .Include(x => x.Employer)
                    .ThenInclude(x => x.Industry)
                    .Include(x => x.Roles)
                    .Include(x => x.Skills)
                    .ThenInclude(x => x.PersonProfileSkill)
                    .FirstAsync(x => x.Id == employment.Id);

                return employment.ToDto();
            }
            else if (details is AssignmentDetailsDto assignmentDetails)
            {

            } 
            else if (details is ProjectDetailsDto projectDetails)
            {

            } 
            else if (details is RoleDetailsDto roleDetails)
            {
                var employment = await context.Employments
                    .Include(x => x.Assignments)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x => x.Id == roleDetails.EmploymentId, cancellationToken);

                var assignment = employment.Assignments
                    .FirstOrDefault(x => x.Id == roleDetails.AssignmentId);

                var role = new EmploymentRole
                {
                    PersonProfile = personProfile,
                    Assignment = assignment,
                    Title = roleDetails.Title,
                    Description = roleDetails.Description,
                    StartDate = roleDetails.StartDate,
                    EndDate = roleDetails.EndDate,
                };

                employment.Roles.Add(role);

                personProfile.Employments.Add(employment);

                await context.SaveChangesAsync(cancellationToken);

                employment = await context.Employments
                    .Include(x => x.Assignments)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x => x.Id == roleDetails.EmploymentId, cancellationToken);

                return employment.ToDto();
            }
            else if (details is CareerBreakDetailsDto careerBreak)
            {

            }

            throw new Exception();
        }
    }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(EmploymentDetailsDto), "Employment")]
[JsonDerivedType(typeof(AssignmentDetailsDto), "Assignment")]
[JsonDerivedType(typeof(ProjectDetailsDto), "Project")]
[JsonDerivedType(typeof(RoleDetailsDto), "Role")]
[JsonDerivedType(typeof(CareerBreakDetailsDto), "CareerBreak")]
public abstract record ExperienceDetailsDto(
    ExperienceType Type,
    DateTime StartDate, DateTime? EndDate,
    string? Description);

public record CareerBreakDetailsDto(
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : ExperienceDetailsDto(ExperienceType.CareerBreak, StartDate, EndDate, Description);

public record EmploymentDetailsDto(
    string Title,
    string CompanyId,
    string? Location,
    EmploymentType EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : ExperienceDetailsDto(ExperienceType.Employment, StartDate, EndDate, Description);

public record AssignmentDetailsDto(
    string Title,
    string EmploymentId,
    string? CompanyId,
    string? Location,
    AssignmentType AssignmentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : ExperienceDetailsDto(ExperienceType.Employment, StartDate, EndDate, Description);

public record ProjectDetailsDto(
    string Title,
    string? EmploymentId,
    string? AssignmentId,
    string? CompanyId,
    string? Location,
    string EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : ExperienceDetailsDto(ExperienceType.Employment, StartDate, EndDate, Description);

public record RoleDetailsDto(
    string Title,
    string EmploymentId,
    string? AssignmentId,
    string? CompanyId,
    string? Location,
    string EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : ExperienceDetailsDto(ExperienceType.Employment, StartDate, EndDate, Description);