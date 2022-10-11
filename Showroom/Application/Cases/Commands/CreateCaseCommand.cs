using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Cases.Commands;

public record CreateCaseCommand(string? Description) : IRequest<CaseDto>
{
    public class CreateCaseCommandHandler : IRequestHandler<CreateCaseCommand, CaseDto>
    {
        private readonly IShowroomContext context;
        private readonly IUrlHelper _urlHelper;

        public CreateCaseCommandHandler(IShowroomContext context, IUrlHelper urlHelper)
        {
            this.context = context;
            _urlHelper = urlHelper;
        }

        public async Task<CaseDto> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
        {
            var @case = new Domain.Entities.Case
            {
                Id = Guid.NewGuid().ToString(),
                Status = Domain.Enums.CaseStatus.Created,
                Description = request.Description
            };

            context.Cases.Add(@case);

            await context.SaveChangesAsync(cancellationToken);

            @case = await context.Cases          
                .Include(c => c.CaseProfiles)
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .FirstOrDefaultAsync(x => x.Id == @case.Id);

            return @case.ToDto(_urlHelper);
        }
    }
}
