
using MediatR;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Addresses.Commands;

public record CreateAddress(string FirstName, string LastName, string SSN) : IRequest<AddressDto>
{
    public class Handler(IMarketingContext context) : IRequestHandler<CreateAddress, AddressDto>
    {
        public async Task<AddressDto> Handle(CreateAddress request, CancellationToken cancellationToken)
        {
            var person = new Domain.Entities.Address(Domain.Enums.AddressType.Delivery);

            context.Addresses.Add(person);

            await context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
        }
    }
}