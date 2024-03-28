using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.Organizations.Commands;

public record CreateOrganizationCommand(string Id, string Name, string? ParentOrganizationId) : IRequest<OrganizationDto>
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrganizationCommandHandler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork)
        {
            _organizationRepository = organizationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _organizationRepository.GetOrganizationByName(request.Name, cancellationToken);

            if (organization is not null) throw new Exception();

            organization = new Domain.Entities.Organization(request.Id, request.Name, null);

            var parentOrg = request.ParentOrganizationId is null ? null : await _organizationRepository.GetOrganizationById(request.ParentOrganizationId);

            parentOrg?.AddSubOrganization(organization);

            _organizationRepository.AddOrganization(organization);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}
