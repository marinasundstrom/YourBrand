
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application.Persons.Queries;

public record GetPersonsQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, HumanResources.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<PersonDto>>
{
    public class GetPersonsQueryHandler : IRequestHandler<GetPersonsQuery, ItemsResult<PersonDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPersonsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<PersonDto>> Handle(GetPersonsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Persons
                .OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(request.SearchString.ToLower())
                    || p.LastName.ToLower().Contains(request.SearchString.ToLower())
                    || ((p.DisplayName ?? "").ToLower().Contains(request.SearchString.ToLower()))
                    || p.SSN.ToLower().Contains(request.SearchString.ToLower())
                    || p.Email.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == HumanResources.Application.Common.Models.SortDirection.Desc ? HumanResources.Application.SortDirection.Descending : HumanResources.Application.SortDirection.Ascending);
            }

            var persons = await query
                .Include(u => u.Roles)
                .Include(u => u.Department)
                .Include(u => u.ReportsTo)
                .ToListAsync(cancellationToken);

            var dtos = persons.Select(person => person.ToDto());

            return new ItemsResult<PersonDto>(dtos, totalItems);
        }
    }
}