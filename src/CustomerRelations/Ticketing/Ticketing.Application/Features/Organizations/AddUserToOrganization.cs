using FluentValidation;

using MediatR;

using YourBrand.Ticketing.Domain;
using YourBrand.Ticketing.Domain.Repositories;
using YourBrand.Ticketing.Application.Features.Organizations;

namespace YourBrand.Ticketing.Application.Features.Organizations;

public record AddUserToOrganization(string OrganizationId, string UserId) : IRequest<Result<OrganizationDto>>
{
    public class Validator : AbstractValidator<AddUserToOrganization>
    {
        public Validator()
        {
        }
    }

    public class Handler(IOrganizationRepository organizationRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddUserToOrganization, Result<OrganizationDto>>
    {
        public async Task<Result<OrganizationDto>> Handle(AddUserToOrganization request, CancellationToken cancellationToken)
        {
            var organization = await organizationRepository.FindByIdAsync(request.OrganizationId!, cancellationToken);

            if (organization is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            var user = await userRepository.FindByIdAsync(request.UserId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<OrganizationDto>(Errors.Users.UserNotFound);
            }

            if (organization.Users.Contains(user))
            {
                return Result.Success(organization.ToDto());
            }

            organization.Users.Add(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(organization.ToDto());
        }
    }
}