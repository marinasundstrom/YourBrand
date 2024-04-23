using MediatR;

using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Showroom.Domain;
using YourBrand.Showroom.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Showroom.Application.Organizations.Commands;

public record AddUserToOrganization(string OrganizationId, string UserId) : IRequest<OrganizationDto>
{
    public class Handler(IShowroomContext context) : IRequestHandler<AddUserToOrganization, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(AddUserToOrganization request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations.FirstOrDefaultAsync(i => i.Id == request.OrganizationId, cancellationToken);

            if (organization is not null) throw new Exception();

            var user = await context.Users
                                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new Exception();
            }

            if (organization.Users.Contains(user))
            {
                throw new Exception();

                //return Result.Success(organization.ToDto());
            }

            organization.AddUser(user);

            await context.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}