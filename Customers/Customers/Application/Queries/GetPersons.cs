using YourBrand.Customers.Domain;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Application.Queries;

public record GetPersons(int Page = 1, int PageSize = 10) : IRequest<ItemsResult<PersonDto>>
{
    public class Handler : IRequestHandler<GetPersons, ItemsResult<PersonDto>>
    {
        private readonly ICustomersContext _context;

        public Handler(ICustomersContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<PersonDto>> Handle(GetPersons request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Persons
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            int totalItems = await query.CountAsync(cancellationToken);

            query = query         
                .Include(i => i.Addresses)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<PersonDto>(
                items.Select(invoice => invoice.ToDto()),
                totalItems);
        }
    }
}