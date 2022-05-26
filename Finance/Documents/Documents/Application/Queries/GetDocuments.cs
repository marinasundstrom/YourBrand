using System.Linq;

using Documents.Application.Common.Models;
using Documents.Infrastructure.Persistence;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Documents.Application.Queries;

public record GetDocuments(int Page, int PageSize) : IRequest<ItemsResult<DocumentDto>>
{
    public class Handler : IRequestHandler<GetDocuments, ItemsResult<DocumentDto>>
    {
        private readonly DocumentsContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(DocumentsContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ItemsResult<DocumentDto>> Handle(GetDocuments request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }
            
            var query = _context.Documents
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
                items.Select(document => document.ToDto(GetUrl(document.BlobId))),
                totalItems);
        }

        private string GetUrl(string blobId)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            return $"{request.Scheme}://{request.Host}/documents/{blobId}";
        }
    }
}
