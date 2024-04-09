
using MediatR;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Addresses.Commands;

public record CreateAddress(string FirstName, string LastName, string SSN) : IRequest<AddressDto>
{
    public class Handler(ICustomersContext context) : IRequestHandler<CreateAddress, AddressDto>
    {
        public async Task<AddressDto> Handle(CreateAddress request, CancellationToken cancellationToken)
        {
            var person = new Domain.Entities.Address();

            context.Addresses.Add(person);

            await context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
        }
    }
}