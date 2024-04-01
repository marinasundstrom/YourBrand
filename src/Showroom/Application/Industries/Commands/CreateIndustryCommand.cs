using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Industries.Commands;

public record CreateIndustryCommand(string Name) : IRequest<IndustryDto>
{
    public class CreateIndustryCommandHandler : IRequestHandler<CreateIndustryCommand, IndustryDto>
    {
        private readonly IShowroomContext context;

        public CreateIndustryCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<IndustryDto> Handle(CreateIndustryCommand request, CancellationToken cancellationToken)
        {
            var industry = await context.Industries.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (industry is not null) throw new Exception();

            industry = new Domain.Entities.Industry
            {
                Name = request.Name
            };

            context.Industries.Add(industry);

            await context.SaveChangesAsync(cancellationToken);

            industry = await context
               .Industries
               .AsNoTracking()
               .FirstAsync(c => c.Id == industry.Id);

            return industry.ToDto();
        }
    }
}