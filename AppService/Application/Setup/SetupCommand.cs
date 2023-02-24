
using System.Data.Common;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain.Entities;
using YourBrand.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.HumanResources.Contracts;
using MassTransit;

namespace YourBrand.Application.Setup;

public record SetupCommand(string OrganizationName, string Email, string Password) : IRequest
{
    public class SetupCommandHandler : IRequestHandler<SetupCommand>
    {
        private readonly IRequestClient<CreateOrganization> _createOrgClient;
        private readonly IRequestClient<CreatePerson> _createPersonClient;

        public SetupCommandHandler(IRequestClient<CreateOrganization> createOrgClient, IRequestClient<CreatePerson> createPersonClient)
        {
            _createOrgClient = createOrgClient;
            _createPersonClient = createPersonClient;
        }

        public async Task Handle(SetupCommand request, CancellationToken cancellationToken)
        {
            var res = await _createOrgClient.GetResponse<CreateOrganizationResponse>(new CreateOrganization {
                Name = request.OrganizationName, 
                FriendlyName = null
            });

            await _createPersonClient.GetResponse<CreatePersonResponse>(new CreatePerson {
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