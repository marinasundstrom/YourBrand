
using YourBrand.Marketing.Domain;

using MediatR;
using YourBrand.Marketing.Application.Prospects;

namespace YourBrand.Marketing.Application.Prospects.Commands;

public record CreateProspect(string FirstName, string LastName, string SSN) : IRequest<ProspectDto>
{
    public class Handler : IRequestHandler<CreateProspect, ProspectDto>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ProspectDto> Handle(CreateProspect request, CancellationToken cancellationToken)
        {
            /*
            var person = new Domain.Entities.Prospect(request.FirstName, request.LastName, request.SSN);

            _context.Prospects.Add(person);

            await _context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
            */

            return null!;
        }
    }
}
