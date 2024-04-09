using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.Organizations
.Commands;

public record DeleteOrganizationCommand(string Id) : IRequest
{
    public class DeleteOrganizationCommandHandler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteOrganizationCommand>
    {
        public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await organizationRepository.GetOrganizationById(request.Id, cancellationToken);

            if (organization is null) throw new Exception();

            organizationRepository.RemoveOrganization(organization);

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}