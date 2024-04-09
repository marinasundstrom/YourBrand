using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.Organizations.Commands;

public record UpdateOrganizationCommand(string Id, string Name) : IRequest<OrganizationDto>
{
    public class UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await organizationRepository.GetOrganizationById(request.Id, cancellationToken);

            if (organization is null) throw new Exception();

            organization.Name = request.Name;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}