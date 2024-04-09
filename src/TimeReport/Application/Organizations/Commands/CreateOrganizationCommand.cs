using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.Organizations.Commands;

public record CreateOrganizationCommand(string Id, string Name, string? ParentOrganizationId) : IRequest<OrganizationDto>
{
    public class CreateOrganizationCommandHandler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await organizationRepository.GetOrganizationByName(request.Name, cancellationToken);

            if (organization is not null) throw new Exception();

            organization = new Domain.Entities.Organization(request.Id, request.Name, null);

            var parentOrg = request.ParentOrganizationId is null ? null : await organizationRepository.GetOrganizationById(request.ParentOrganizationId);

            parentOrg?.AddSubOrganization(organization);

            organizationRepository.AddOrganization(organization);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}