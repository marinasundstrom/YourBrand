using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Companies.Commands;

public record DeleteCompanyCommand(string Id) : IRequest
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand>
    {
        private readonly IShowroomContext context;

        public DeleteCompanyCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await context.Companies
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (company is null) throw new Exception();

            context.Companies.Remove(company);
           
            await context.SaveChangesAsync(cancellationToken);

        }
    }
}