
using MediatR;

using YourBrand.Customers.Application.Addresses;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Application.Persons.Commands;

public record CreatePerson(string FirstName, string LastName, string SSN, string? Phone, string? PhoneMobile, string? Email, Address2Dto Address) : IRequest<PersonDto>
{
    public class Handler(ICustomersContext context) : IRequestHandler<CreatePerson, PersonDto>
    {
        public async Task<PersonDto> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = new Domain.Entities.Person(request.FirstName, request.LastName, request.SSN);
            person.Phone = request.Phone;
            person.PhoneMobile = request.PhoneMobile!;
            person.Email = request.Email!;

            person.AddAddress(new Address
            {
                Thoroughfare = request.Address.Thoroughfare,
                Premises = request.Address.Premises,
                SubPremises = request.Address.SubPremises,
                PostalCode = request.Address.PostalCode,
                Locality = request.Address.Locality,
                SubAdministrativeArea = request.Address.SubAdministrativeArea,
                AdministrativeArea = request.Address.AdministrativeArea,
                Country = request.Address.Country
            });

            context.Persons.Add(person);

            await context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
        }
    }
}