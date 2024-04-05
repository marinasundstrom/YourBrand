using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Tenants.Commands;

public record DeleteTenantCommand(string TenantId) : IRequest
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteTenantCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IEventPublisher _eventPublisher;

        public DeleteUserCommandHandler(IApplicationDbContext context, IUserContext userContext, IEventPublisher eventPublisher)
        {
            _context = context;
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(p => p.Id == request.TenantId);

            if (tenant is null)
            {
                throw new UserNotFoundException(request.TenantId);
            }

            _context.Tenants.Remove(tenant);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new TenantDeleted(tenant.Id, _userContext.UserId));

        }
    }
}