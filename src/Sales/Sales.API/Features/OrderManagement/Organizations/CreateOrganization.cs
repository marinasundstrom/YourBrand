using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Organizations;

public record CreateOrganization(string Id, string Name) : IRequest<Result<OrganizationDto>>
{
    public class Validator : AbstractValidator<CreateOrganization>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler : IRequestHandler<CreateOrganization, Result<OrganizationDto>>
    {
        private readonly IOrganizationRepository organizationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUserService;

        public Handler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.organizationRepository = organizationRepository;
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<OrganizationDto>> Handle(CreateOrganization request, CancellationToken cancellationToken)
        {
            organizationRepository.Add(new Organization(request.Id, request.Name));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var organization = await organizationRepository.FindByIdAsync(request.Id, cancellationToken);

            if (organization is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            return Result.Success(organization.ToDto2());
        }
    }
}