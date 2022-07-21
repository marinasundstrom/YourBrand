
using System.Data.Common;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain.Entities;
using YourBrand.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.HumanResources.Contracts;

namespace YourBrand.Application.Setup;

public record SetupCommand(string OrganizationName, string Email, string Password) : IRequest
{
    public class SetupCommandHandler : IRequestHandler<SetupCommand>
    {
        public SetupCommandHandler(/*IRequestClient<CreateOrganization> */)
        {
            
        }

        public async Task<Unit> Handle(SetupCommand request, CancellationToken cancellationToken)
        {
            return Unit.Value;
        }
    }
}