
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

        public async Task<Unit> Handle(SetupCommand request, CancellationToken cancellationToken)
        {
            var res = await _createOrgClient.GetResponse<GetOrganizationResponse>(new CreateOrganization(request.OrganizationName, null));
            await _createOrgClient.GetResponse<GetPersonResponse>(new CreatePerson(res.Message.Id, "Administrator", "Administrator", "Administrator", "Administrator", "Administrator", "234234", request.Email, null!, null, request.Password));

            return Unit.Value; 
        }
    }
}