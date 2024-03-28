
using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.Identity;

using OrganizationCreated = YourBrand.HumanResources.Contracts.OrganizationCreated;

namespace YourBrand.HumanResources.Application.Organizations.Commands;

public record CreateOrganizationCommand(string Name, string? FriendlyName) : IRequest<OrganizationDto>
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentOrganizationService;
        private readonly IEventPublisher _eventPublisher;

        public CreateOrganizationCommandHandler(IApplicationDbContext context, ICurrentUserService currentOrganizationService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentOrganizationService = currentOrganizationService;
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

            await _eventPublisher.PublishEvent(new OrganizationCreated(organization.Id, organization.Name, _currentOrganizationService.UserId));

            return organization.ToDto();
        }
    }
}