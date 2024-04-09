using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Cases.Commands;

public record DeleteCaseCommand(string Id) : IRequest
{
    public class DeleteCaseCommandHandler(IShowroomContext context) : IRequestHandler<DeleteCaseCommand>
    {
        public async Task Handle(DeleteCaseCommand request, CancellationToken cancellationToken)
        {
            var comepetenceArea = await context.Cases
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (comepetenceArea is null) throw new Exception();

            context.Cases.Remove(comepetenceArea);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}