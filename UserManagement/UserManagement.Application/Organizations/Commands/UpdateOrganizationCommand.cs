using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Application.Users;
using YourBrand.UserManagement.Contracts;
using YourBrand.UserManagement.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Application.Organizations.Commands;

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
