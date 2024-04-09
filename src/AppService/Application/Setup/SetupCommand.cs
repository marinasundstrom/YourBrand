using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;

namespace YourBrand.Application.Setup;

public record SetupCommand(string OrganizationName, string Email, string Password) : IRequest
{
    public class SetupCommandHandler(IRequestClient<CreateOrganization> createOrgClient, IRequestClient<CreatePerson> createPersonClient) : IRequestHandler<SetupCommand>
    {
        public async Task Handle(SetupCommand request, CancellationToken cancellationToken)
        {
            var res = await createOrgClient.GetResponse<CreateOrganizationResponse>(new CreateOrganization
            {
                Name = request.OrganizationName,
                FriendlyName = null
            });

            await createPersonClient.GetResponse<CreatePersonResponse>(new CreatePerson
            {
                OrganizationId = res.Message.Id,
                FirstName = "Administrator",
                LastName = "Administrator",
                DisplayName = "Administrator",
                Title = "Administrator",
                Role = "Administrator",
                SSN = "234234",
                Email = request.Email,
                DepartmentId = null!,
                ReportsTo = null,
                Password = request.Password
            });

        }
    }
}