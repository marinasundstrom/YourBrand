using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Cases.Commands;

public record UpdateCaseCommand(string Id, string? Description) : IRequest
{
    public class UpdateCaseCommandHandler(IShowroomContext context) : IRequestHandler<UpdateCaseCommand>
    {
        public async Task Handle(UpdateCaseCommand request, CancellationToken cancellationToken)
        {
            var @case = await context.Cases.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (@case is null) throw new Exception();

            @case.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}