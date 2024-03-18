using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Contracts;
using YourBrand.UserManagement.Domain.Entities;
using YourBrand.UserManagement.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Application.Organizations.Commands;

public record DeleteOrganizationCommand(string OrganizationId) : IRequest
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteOrganizationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public DeleteUserCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentUserService = currentUserService;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(p => p.Id == request.OrganizationId);

            if (organization is null)
            {
                throw new UserNotFoundException(request.OrganizationId);
            }

            _context.Organizations.Remove(organization);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new OrganizationDeleted(organization.Id, _currentUserService.UserId));

        }
    }
}