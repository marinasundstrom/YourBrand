using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.Cases.Commands;

public record CasePricing(decimal? HourlyPrice, double? Hours);

public record CreateCaseCommand(string OrganizationId, string? Description, string? PersonProfileId, CasePricing? Pricing) : IRequest<CaseDto>
{
    public class CreateCaseCommandHandler(IShowroomContext context, IUrlHelper urlHelper) : IRequestHandler<CreateCaseCommand, CaseDto>
    {
        public async Task<CaseDto> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
        {
            var @case = new Domain.Entities.Case
            {
                OrganizationId = request.OrganizationId,
                Status = Domain.Enums.CaseStatus.Created,
                Description = request.Description
            };

            if (request.PersonProfileId is not null)
            {
                var personProfile = context.PersonProfiles.FirstOrDefault(x => x.Id == request.PersonProfileId);

                if (personProfile is not null)
                {
                    var candidateProfile = new CaseCandidateProfile
                    {
                        OrganizationId = request.OrganizationId,
                        PersonProfile = personProfile,
                        Presentation = ""
                    };
                    @case.CandidateProfiles.Add(candidateProfile);
                }
            }

            var pricing = request.Pricing;

            if (pricing is not null)
            {
                @case.Pricing = new Domain.Entities.CasePricing
                {
                    HourlyPrice = pricing.HourlyPrice,
                    Hours = pricing.Hours,
                    Total = pricing.HourlyPrice * (decimal?)pricing.Hours
                };
            }

            context.Cases.Add(@case);

            await context.SaveChangesAsync(cancellationToken);

            @case = await context.Cases
                .Include(c => c.CandidateProfiles)
                .ThenInclude(c => c.PersonProfile)
                .ThenInclude(c => c.Organization)
                .Include(c => c.CandidateProfiles)
                .ThenInclude(c => c.PersonProfile)
                .ThenInclude(c => c.Industry)
                .Include(c => c.CandidateProfiles)
                .ThenInclude(c => c.PersonProfile)
                .ThenInclude(c => c.CompetenceArea)
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .FirstOrDefaultAsync(x => x.Id == @case.Id);

            return @case.ToDto(urlHelper);
        }
    }
}