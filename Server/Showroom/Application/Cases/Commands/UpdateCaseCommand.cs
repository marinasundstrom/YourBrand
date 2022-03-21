using MediatR;

using Microsoft.EntityFrameworkCore;
using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Application.Cases.Commands;

public record UpdateCaseCommand(string Id, string? Description) : IRequest
{
    public class UpdateCaseCommandHandler : IRequestHandler<UpdateCaseCommand>
    {
        private readonly IShowroomContext context;

        public UpdateCaseCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateCaseCommand request, CancellationToken cancellationToken)
        {
            var @case = await context.Cases.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (@case is null) throw new Exception();

            @case.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
