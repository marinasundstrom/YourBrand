using FluentValidation;

using MediatR;

using YourBrand.Sales.API.Features.OrderManagement.Orders;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.API.Features.OrderManagement.Organizations;

public record DeleteOrganization(string OrganizationId) : IRequest<Result<DeleteOrganization>>
{
    public class Validator : AbstractValidator<DeleteOrganization>
    {
        public Validator()
        {
        }
    }

    public class Handler : IRequestHandler<DeleteOrganization, Result>
    {
        private readonly IOrganizationRepository organizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService currentUserService;

        public Handler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.organizationRepository = organizationRepository;
            _unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteOrganization request, CancellationToken cancellationToken)
        {
            var user = await organizationRepository.FindByIdAsync(request.OrganizationId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            organizationRepository.Remove(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto2());
        }
    }
}
