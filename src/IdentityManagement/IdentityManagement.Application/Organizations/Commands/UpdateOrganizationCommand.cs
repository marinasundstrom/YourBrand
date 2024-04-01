using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Organizations.Commands;

public record UpdateOrganizationCommand(string OrganizationId, string Name) : IRequest<OrganizationDto>
{
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public UpdateUserDetailsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentUserService = currentUserService;
            _eventPublisher = eventPublisher;
        }

        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _context.Organizations
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is null)
            {
                throw new UserNotFoundException(request.OrganizationId);
            }

            organization.ChangeName(request.Name);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new OrganizationUpdated(organization.Id, organization.Name, _currentUserService.UserId));

            return organization.ToDto();
        }
    }
}