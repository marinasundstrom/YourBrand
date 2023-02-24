using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.Organizations
.Commands;

public record DeleteOrganizationCommand(string Id) : IRequest
{
    public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand>
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrganizationCommandHandler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork)
        {
            _organizationRepository = organizationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _organizationRepository.GetOrganizationById(request.Id, cancellationToken);

            if (organization is null) throw new Exception();

            _organizationRepository.RemoveOrganization(organization);
           
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}