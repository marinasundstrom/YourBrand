using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Tenants.Commands;

public record UpdateTenantCommand(string TenantId, string Name) : IRequest<TenantDto>
{
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateTenantCommand, TenantDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IEventPublisher _eventPublisher;

        public UpdateUserDetailsCommandHandler(IApplicationDbContext context, IUserContext userContext, IEventPublisher eventPublisher)
        {
            _context = context;
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }

        public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _context.Tenants
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TenantId, cancellationToken);

            if (tenant is null)
            {
                throw new UserNotFoundException(request.TenantId);
            }

            tenant.ChangeName(request.Name);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new TenantUpdated(tenant.Id, tenant.Name, _userContext.UserId));

            return tenant.ToDto();
        }
    }
}