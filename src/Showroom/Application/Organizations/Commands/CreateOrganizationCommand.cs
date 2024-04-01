using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Organizations.Commands;

public record CreateOrganizationCommand(string Id, string Name) : IRequest
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand>
    {
        private readonly IShowroomContext context;

        public CreateOrganizationCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (organization is not null) throw new Exception();

            organization = new Domain.Entities.Organization
            {
                Id = request.Id ?? request.Name.ToLower().Replace(' ', '-'),
                Name = request.Name
            };

            context.Organizations.Add(organization);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}