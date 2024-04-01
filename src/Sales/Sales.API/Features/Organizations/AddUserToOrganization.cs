using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Persistence.Repositories.Mocks;

namespace YourBrand.Sales.Features.OrderManagement.Organizations;

public record AddUserToOrganization(string OrganizationId, string UserId) : IRequest<Result<OrganizationDto>>
{
    public class Validator : AbstractValidator<AddUserToOrganization>
    {
        public Validator()
        {
        }
    }

    public class Handler : IRequestHandler<AddUserToOrganization, Result<OrganizationDto>>
    {
        private readonly IOrganizationRepository organizationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService currentUserService;

        public Handler(IOrganizationRepository organizationRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<OrganizationDto>> Handle(AddUserToOrganization request, CancellationToken cancellationToken)
        {
            var organization = await organizationRepository.FindByIdAsync(request.OrganizationId!, cancellationToken);

            if (organization is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            var user = await _userRepository.FindByIdAsync(request.UserId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Users.UserNotFound);
            }

            if(organization.Users.Contains(user)) 
            {
                return Result.Success(organization.ToDto());
            }

            organization.Users.Add(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(organization.ToDto());
        }
    }
}