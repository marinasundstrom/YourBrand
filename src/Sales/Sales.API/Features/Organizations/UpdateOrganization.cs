using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Services;

namespace YourBrand.Sales.Features.OrderManagement.Organizations;

public record UpdateOrganization(string OrganizationId, string Name) : IRequest<Result<OrganizationDto>>
{
    public class Validator : AbstractValidator<UpdateOrganization>
    {
        public Validator()
        {
        }
    }

    public class Handler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<UpdateOrganization, Result<OrganizationDto>>
    {
        public async Task<Result<OrganizationDto>> Handle(UpdateOrganization request, CancellationToken cancellationToken)
        {
            var user = await organizationRepository.FindByIdAsync(request.OrganizationId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            user.Name = request.Name;
            //user.Fr = request.Name;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto());
        }
    }
}
