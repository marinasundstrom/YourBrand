
using YourBrand.Customers.Domain;

using MediatR;
using YourBrand.Customers.Application.Persons;

namespace YourBrand.Customers.Application.Persons.Commands;

public record CreatePerson(string FirstName, string LastName, string SSN, string? Phone, string? PhoneMobile, string? Email) : IRequest<PersonDto>
{
    public class Handler : IRequestHandler<CreatePerson, PersonDto>
    {
        private readonly ICustomersContext _context;

        public Handler(ICustomersContext context)
        {
            _context = context;
        }

        public async Task<PersonDto> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = new Domain.Entities.Person(request.FirstName, request.LastName, request.SSN);
            person.Phone = request.Phone;
            person.PhoneMobile = request.PhoneMobile!;
            person.Email = request.Email!;
            
            _context.Persons.Add(person);

            await _context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
        }
    }
}
