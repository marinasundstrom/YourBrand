using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.PersonProfiles.Experiences;
using YourBrand.Showroom.Events.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Employments.Commands;

public record UpdateEmploymentCommand(
    string PersonProfileId,
    string Id,
    string Title,
    string CompanyId,
    string? Location,
    string EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : IRequest<EmploymentDto>
{
    sealed class UpdateEmploymentCommandHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<UpdateEmploymentCommand, EmploymentDto>
    {
        public async Task<EmploymentDto> Handle(UpdateEmploymentCommand request, CancellationToken cancellationToken)
        {
            var employment = await context.Employments
                .Include(x => x.PersonProfile)
                .Include(x => x.Employer)
                .ThenInclude(x => x.Industry)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            if (employment is null)
            {
                throw new Exception("Not found");
            }

            //employment.Title = request.Title;
            //employment.CompanyId = request.CompanyId;
            //employment.EmploymentType = request.EmploymentType;
            //employment.Location = request.Location;
            employment.StartDate = request.StartDate;
            employment.EndDate = request.EndDate;
            employment.Description = request.Description;

            //employment.AddDomainEvent(new EmploymentUpdated(employment.PersonProfile.Id, employment.PersonProfile.Id, employment.Company.Industry.Id));

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