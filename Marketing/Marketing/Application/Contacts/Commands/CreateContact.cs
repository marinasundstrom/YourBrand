
using YourBrand.Marketing.Domain;

using MediatR;
using YourBrand.Marketing.Application.Contacts;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record CreateContact(string FirstName, string LastName, string SSN) : IRequest<ContactDto>
{
    public class Handler : IRequestHandler<CreateContact, ContactDto>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ContactDto> Handle(CreateContact request, CancellationToken cancellationToken)
        {
            /*
            var person = new Domain.Entities.Contact(request.FirstName, request.LastName, request.SSN);

            _context.Contacts.Add(person);

            await _context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
            */

            return null!;
        }
    }
}
