using MediatR;

using Microsoft.EntityFrameworkCore;
using Skynet.Showroom.Application.Common.Interfaces;

namespace Skynet.Showroom.Application.Cases.Commands;

public record CreateCaseCommand(string? Description) : IRequest
{
    public class CreateCaseCommandHandler : IRequestHandler<CreateCaseCommand>
    {
        private readonly IShowroomContext context;

        public CreateCaseCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
        {
            var @case = await context.Cases.FirstOrDefaultAsync(i => i.Description == request.Description, cancellationToken);

            if (@case is not null) throw new Exception();

            @case = new Domain.Entities.Case
            {
                Description = request.Description
            };

            context.Cases.Add(@case);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
