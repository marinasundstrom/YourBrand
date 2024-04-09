using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Infrastructure.Persistence;

namespace YourBrand.Documents.Application.Queries;

public record GetDocuments(int Page, int PageSize) : IRequest<ItemsResult<DocumentDto>>
{
    public class Handler(DocumentsContext context, IUrlResolver urlResolver) : IRequestHandler<GetDocuments, ItemsResult<DocumentDto>>
    {
        public async Task<ItemsResult<DocumentDto>> Handle(GetDocuments request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.Documents
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Created)
                .AsQueryable();

            int totalItems = await query.CountAsync(cancellationToken);

            query = query
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<DocumentDto>(
                items.Select(document => document.ToDto(urlResolver.GetUrl)),
                totalItems);
        }
    }
}