using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.Tenancy;

namespace YourBrand.Application.Setup;

public record SetupCommand(string OrganizationName, string Email, string Password) : IRequest
{
    public class SetupCommandHandler(ISettableTenantContext tenantContext, IRequestClient<CreateTenant> createTenantClient, IRequestClient<CreateOrganization> createOrgClient, IRequestClient<CreateUser> createUserClient) : IRequestHandler<SetupCommand>
    {
        public async Task Handle(SetupCommand request, CancellationToken cancellationToken)
        {
            var tenantRes = await createTenantClient.GetResponse<CreateTenantResponse>(new CreateTenant
            {
                Name = request.OrganizationName,
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
                FirstName = "Administrator",
                LastName = "Administrator",
                DisplayName = "Administrator",
                Role = "Administrator",
                Email = request.Email,
                Password = request.Password
            });

        }
    }
}