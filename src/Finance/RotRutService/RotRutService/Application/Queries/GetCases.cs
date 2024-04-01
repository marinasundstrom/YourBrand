using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Domain.Entities;
using YourBrand.RotRutService.Domain.Enums;

namespace YourBrand.RotRutService.Application.Queries;

public record GetCases(int Page = 0, int PageSize = 10, DomesticServiceKind? Kind = null, RotRutCaseStatus[]? Status = null) : IRequest<ItemsResult<RotRutCaseDto>>
{
    public class Handler : IRequestHandler<GetCases, ItemsResult<RotRutCaseDto>>
    {
        private readonly IRotRutContext _context;

        public Handler(IRotRutContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<RotRutCaseDto>> Handle(GetCases request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.RotRutCases
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            if (request.Kind is not null)
            {
                query = query.Where(i => i.Kind == request.Kind);
            }

            if (request.Status?.Any() ?? false)
            {
                var statuses = request.Status.Select(x => (int)x);
                query = query.Where(i => statuses.Any(s => s == (int)i.Status));
            }

            int totalItems = await query.CountAsync(cancellationToken);

            query = query
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<RotRutCaseDto>(
                items.Select(rotRutCase => rotRutCase.ToDto()),
                totalItems);
        }
    }
}