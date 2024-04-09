using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Companies.Commands;

public record CreateCompanyCommand(string Name, int IndustryId) : IRequest<CompanyDto>
{
    public class CreateCompanyCommandHandler(IShowroomContext context) : IRequestHandler<CreateCompanyCommand, CompanyDto>
    {
        public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await context.Companies.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (company is not null) throw new Exception();

            company = new Domain.Entities.Company
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Industry = await context.Industries.FirstAsync(x => x.Id == request.IndustryId, cancellationToken)
            };

            context.Companies.Add(company);

            await context.SaveChangesAsync(cancellationToken);

            company = await context
               .Companies
               .Include(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.Id == company.Id);

            return company.ToDto();
        }
    }
}