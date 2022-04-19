using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Organizations.Commands;

public record CreateOrganizationCommand(string Name) : IRequest<OrganizationDto>
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        private readonly ITimeReportContext context;

        public CreateOrganizationCommandHandler(ITimeReportContext context)
        {
            this.context = context;
        }

        public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (organization is not null) throw new Exception();

            organization = new Domain.Entities.Organization
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name
            };

            context.Organizations.Add(organization);

            await context.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}
