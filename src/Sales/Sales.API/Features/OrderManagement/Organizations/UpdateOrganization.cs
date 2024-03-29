using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Organizations;

public record UpdateOrganization(string OrganizationId, string Name) : IRequest<Result<OrganizationDto>>
{
    public class Validator : AbstractValidator<UpdateOrganization>
    {
        public Validator()
        {
        }
    }

    public class Handler : IRequestHandler<UpdateOrganization, Result<OrganizationDto>>
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

        public async Task<Result<OrganizationDto>> Handle(UpdateOrganization request, CancellationToken cancellationToken)
        {
            var user = await organizationRepository.FindByIdAsync(request.OrganizationId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            user.Name = request.Name;
            //user.Fr = request.Name;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto());
        }
    }
}