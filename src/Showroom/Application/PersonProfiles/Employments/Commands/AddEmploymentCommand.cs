using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.PersonProfiles.Experiences;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Employments.Commands;

public record AddEmploymentCommand(
    string PersonProfileId,
    string Title,
    string CompanyId,
    string? Location,
    EmploymentType EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : IRequest<EmploymentDto>
{
    sealed class CreatePersonProfileCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<AddEmploymentCommand, EmploymentDto>
    {
        public async Task<EmploymentDto> Handle(AddEmploymentCommand request, CancellationToken cancellationToken)
        {
            var personProfile = await context.PersonProfiles.FirstAsync(cp => cp.Id == request.PersonProfileId, cancellationToken);

            var company = await context.Companies
                .Include(x => x.Industry)
                .FirstAsync(cp => cp.Id == request.CompanyId, cancellationToken);

            var employment = new Employment
            {
                PersonProfile = personProfile,
                //Title = request.Title,
                Employer = company,
                EmploymentType = request.EmploymentType,
                //Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description
            };

            context.Employments.Add(employment);

            //employment.AddDomainEvent(new EmploymentUpdated(employment.PersonProfile.Id, personProfile.Id, company.Industry.Id));

            await context.SaveChangesAsync(cancellationToken);

            employment = await context.Employments
                .Include(x => x.Employer)
                .ThenInclude(x => x.Industry)
                .Include(x => x.Skills)
                .ThenInclude(x => x.PersonProfileSkill)
                .FirstAsync(x => x.Id == employment.Id);

            return employment.ToDto();
        }
    }
}