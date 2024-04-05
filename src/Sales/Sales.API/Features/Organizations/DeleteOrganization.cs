using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Organizations;

public record DeleteOrganization(string OrganizationId) : IRequest<Result<DeleteOrganization>>
{
    public class Validator : AbstractValidator<DeleteOrganization>
    {
        public Validator()
        {
        }
    }

    public class Handler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<DeleteOrganization, Result>
    {
        public async Task<Result> Handle(DeleteOrganization request, CancellationToken cancellationToken)
        {
            var user = await organizationRepository.FindByIdAsync(request.OrganizationId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            organizationRepository.Remove(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto2());
        }
    }
}