using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Industries.Commands;

public record UpdateIndustryCommand(int Id, string Name) : IRequest
{
    public class UpdateIndustryCommandHandler(IShowroomContext context) : IRequestHandler<UpdateIndustryCommand>
    {
        public async Task Handle(UpdateIndustryCommand request, CancellationToken cancellationToken)
        {
            var industry = await context.Industries.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (industry is null) throw new Exception();

            industry.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}