
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application.Persons.Queries;

public record GetPersonRolesQuery(string PersonId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, HumanResources.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<RoleDto>>
{
    public class GetPersonRolesQueryHandler : IRequestHandler<GetPersonRolesQuery, ItemsResult<RoleDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPersonRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<RoleDto>> Handle(GetPersonRolesQuery request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
                  .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            if (person is null)
            {
                throw new Exception("Person not found");
            }

            var query = _context.Roles
                .Where(x => x.Persons.Any(x => x.Id == request.PersonId))
                //.OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == HumanResources.Application.Common.Models.SortDirection.Desc ? HumanResources.Application.SortDirection.Descending : HumanResources.Application.SortDirection.Ascending);
            }

            var roles = await query
                .ToListAsync(cancellationToken);

            var dtos = roles.Select(role => new RoleDto(role.Id, role.Name));

            return new ItemsResult<RoleDto>(dtos, totalItems);
        }
    }
}