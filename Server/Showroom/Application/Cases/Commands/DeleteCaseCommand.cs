using MediatR;

using Microsoft.EntityFrameworkCore;
using Skynet.Showroom.Application.Common.Interfaces;

namespace Skynet.Showroom.Application.Cases.Commands;

public record DeleteCaseCommand(string Id) : IRequest
{
    public class DeleteCaseCommandHandler : IRequestHandler<DeleteCaseCommand>
    {
        private readonly IShowroomContext context;

        public DeleteCaseCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(DeleteCaseCommand request, CancellationToken cancellationToken)
        {
            var comepetenceArea = await context.Cases
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (comepetenceArea is null) throw new Exception();

            context.Cases.Remove(comepetenceArea);
           
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}