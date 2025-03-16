using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record IsEmailAlreadyRegisteredCommand(string Email) : IRequest<IsEmailAlreadyRegisteredResult>
{
    public class Handler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<IsEmailAlreadyRegisteredCommand, IsEmailAlreadyRegisteredResult>
    {
        public async Task<IsEmailAlreadyRegisteredResult> Handle(IsEmailAlreadyRegisteredCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.NormalizedEmail == request.Email.ToUpper(), cancellationToken);

            return new IsEmailAlreadyRegisteredResult(user is not null);
        }
    }
}

public record IsEmailAlreadyRegisteredResult(bool EmailExists);