using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Industries.Commands;

public record UpdateIndustryCommand(int Id, string Name) : IRequest
{
    public class UpdateIndustryCommandHandler : IRequestHandler<UpdateIndustryCommand>
    {
        private readonly IShowroomContext context;

        public UpdateIndustryCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateIndustryCommand request, CancellationToken cancellationToken)
        {
            var industry = await context.Industries.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (industry is null) throw new Exception();

            industry.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
