
using MediatR;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Addresses.Commands;

public record CreateAddress(string FirstName, string LastName, string SSN) : IRequest<AddressDto>
{
    public class Handler : IRequestHandler<CreateAddress, AddressDto>
    {
        private readonly ICustomersContext _context;

        public Handler(ICustomersContext context)
        {
            _context = context;
        }

        public async Task<AddressDto> Handle(CreateAddress request, CancellationToken cancellationToken)
        {
            var person = new Domain.Entities.Address();

            _context.Addresses.Add(person);

            await _context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
        }
    }
}