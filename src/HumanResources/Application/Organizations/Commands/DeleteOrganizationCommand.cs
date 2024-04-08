using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Organizations.Commands;

public record DeleteOrganizationCommand(string OrganizationId) : IRequest
{
    public class DeletePersonCommandHandler : IRequestHandler<DeleteOrganizationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public DeletePersonCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(p => p.Id == request.OrganizationId);

            if (organization is null)
            {
                throw new PersonNotFoundException(request.OrganizationId);
            }

            _context.Organizations.Remove(organization);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new OrganizationDeleted(organization.Id));

        }
    }
}