using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Cases.Commands;

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
            var @case = new Domain.Entities.Case
            {
                Id = Guid.NewGuid().ToString(),
                Status = Domain.Enums.CaseStatus.Created,
                Description = request.Description
            };

            context.Cases.Add(@case);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
