using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Organizations.Commands;

public record UpdateOrganizationCommand(string OrganizationId, string Name) : IRequest<OrganizationDto>
{
    public class UpdatePersonDetailsCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public UpdatePersonDetailsCommandHandler(IApplicationDbContext context, ICurrentUserService currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _context.Organizations
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is null)
            {
                throw new PersonNotFoundException(request.OrganizationId);
            }

            organization.ChangeName(request.Name);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new OrganizationUpdated(organization.Id, organization.Name, _currentPersonService.UserId));

            return organization.ToDto();
        }
    }
}