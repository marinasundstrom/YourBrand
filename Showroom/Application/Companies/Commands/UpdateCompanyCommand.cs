using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Companies.Commands;

public record UpdateCompanyCommand(string Id, string Name, int IndustryId) : IRequest
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand>
    {
        private readonly IShowroomContext context;

        public UpdateCompanyCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await context.Companies.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (company is null) throw new Exception();

            company.Name = request.Name;
            company.Industry = await context.Industries.FirstAsync(x => x.Id == request.IndustryId, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
