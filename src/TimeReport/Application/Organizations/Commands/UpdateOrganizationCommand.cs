using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.Organizations.Commands;

public record UpdateOrganizationCommand(string Id, string Name) : IRequest<OrganizationDto>
{
    public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork)
        {
            _organizationRepository = organizationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _organizationRepository.GetOrganizationById(request.Id, cancellationToken);

            if (organization is null) throw new Exception();

            organization.Name = request.Name;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}
