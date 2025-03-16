using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.Tenancy;

namespace YourBrand.Application.Setup;

public record SetupCommand(string? TenantName, string OrganizationName, string FirstName, string LastName, string Email, string Password) : IRequest<SetupResult>
{
    public class SetupCommandHandler(ISettableTenantContext tenantContext, IRequestClient<IsEmailAlreadyRegistered> isEmailAlreadyRegisteredClient, IRequestClient<CreateTenant> createTenantClient, IRequestClient<CreateOrganization> createOrgClient, IRequestClient<CreateUser> createUserClient) : IRequestHandler<SetupCommand, SetupResult>
    {
        public async Task<SetupResult> Handle(SetupCommand request, CancellationToken cancellationToken)
        {
            var emailRes = await isEmailAlreadyRegisteredClient.GetResponse<IsEmailAlreadyRegisteredResponse>(new IsEmailAlreadyRegistered
            {
                Email = request.Email,
            });

            if(emailRes.Message.IsEmailRegistered) 
            {
                return new SetupResult(SetupStatusCode.Failed, SetupFailReason.EmailAddressAlreadyRegistered);
            }

            var tenantRes = await createTenantClient.GetResponse<CreateTenantResponse>(new CreateTenant
            {
                Name = request.TenantName ?? request.OrganizationName,
                FriendlyName = null
            });

            tenantContext.SetTenantId(tenantRes.Message.TenantId);

            var orgRes = await createOrgClient.GetResponse<CreateOrganizationResponse>(new CreateOrganization
            {
                Name = request.OrganizationName,
                FriendlyName = null
            });

            await createUserClient.GetResponse<CreateUserResponse>(new CreateUser
            {
                OrganizationId = orgRes.Message.OrganizationId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "Administrator",
                Email = request.Email,
                Password = request.Password
            });

            return new SetupResult(SetupStatusCode.Succeeded, SetupFailReason.None);
        }
    }
}

public record SetupResult(SetupStatusCode Status, SetupFailReason Reason);

public enum SetupStatusCode
{
    Succeeded,
    Failed
}

public enum SetupFailReason
{
    None,
    EmailAddressAlreadyRegistered
}