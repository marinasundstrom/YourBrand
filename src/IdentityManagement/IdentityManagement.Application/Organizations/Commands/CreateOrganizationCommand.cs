using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Domain.Entities;

using OrganizationCreated = YourBrand.IdentityManagement.Contracts.OrganizationCreated;

namespace YourBrand.IdentityManagement.Application.Organizations.Commands;

public record CreateOrganizationCommand(string Name, string? FriendlyName) : IRequest<OrganizationDto>
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IEventPublisher _eventPublisher;

        public CreateOrganizationCommandHandler(IApplicationDbContext context, IUserContext userContext, IEventPublisher eventPublisher)
        {
            _context = context;
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }

        public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = new Organization(
                 request.Name,
                 request.FriendlyName ?? request.Name.ToLower().Replace(' ', '-'));

            _context.Organizations.Add(organization);

            await _context.SaveChangesAsync(cancellationToken);

            organization = await _context.Organizations
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == organization.Id, cancellationToken);

            await _eventPublisher.PublishEvent(new OrganizationCreated(organization.Id, organization.Tenant.Id, organization.Name, _userContext.UserId));

            return organization.ToDto();
        }
    }
}