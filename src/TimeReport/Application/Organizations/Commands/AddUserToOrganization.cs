using MediatR;

using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.Organizations.Commands;

public record AddUserToOrganization(string OrganizationId, string UserId) : IRequest<OrganizationDto>
{
    public class Handler(IOrganizationRepository organizationRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddUserToOrganization, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(AddUserToOrganization request, CancellationToken cancellationToken)
        {
            var organization = await organizationRepository.GetOrganizationById(request.OrganizationId!, cancellationToken);

            if (organization is null)
            {
                throw new Exception();

                //return Result.Failure<OrganizationDto>(Errors.Organizations.OrganizationNotFound);
            }

            var user = await userRepository.GetUser(request.UserId!, cancellationToken);

            if (user is null)
            {
                throw new Exception();

                //return Result.Failure<OrganizationDto>(Errors.Users.UserNotFound);
            }

            if (organization.Users.Contains(user))
            {
                throw new Exception();

                //return Result.Success(organization.ToDto());
            }

            organization.AddUser(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}