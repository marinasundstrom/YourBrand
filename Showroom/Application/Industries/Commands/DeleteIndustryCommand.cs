using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Industries.Commands;

public record DeleteIndustryCommand(int Id) : IRequest
{
    public class DeleteIndustryCommandHandler : IRequestHandler<DeleteIndustryCommand>
    {
        private readonly IShowroomContext context;

        public DeleteIndustryCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(DeleteIndustryCommand request, CancellationToken cancellationToken)
        {
            var industry = await context.Industries
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (industry is null) throw new Exception();

            context.Industries.Remove(industry);
           
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}