using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record DeleteProduct(string IdOrHandle) : IRequest<Result>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<DeleteProduct, Result>
    {
        public async Task<Result> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products.IncludeBasics().FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products.IncludeBasics().FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            using var transaction = await catalogContext.Database.BeginTransactionAsync();

            await catalogContext.SaveChangesAsync(cancellationToken);

            product.Category?.RemoveProduct(product);

            await catalogContext.ProductAttributes
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.AttributeGroups
                .Where(x => x.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ProductOptions
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.Products
                .Where(x => x.ParentId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ProductVariantOptions
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ChoiceOptions
                .Where(x => x.Group!.Product!.Id == product.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(z => z.DefaultValueId, (string?)null), cancellationToken);

            await catalogContext.OptionValues
                .Where(x => x.Option.Group!.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.Options
                .Where(x => x.Group!.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.OptionGroups
                .Where(x => x.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ProductImages
                .Where(x => x.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            catalogContext.Products.Remove(product);

            await catalogContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync();

            return Result.Success();
        }
    }
}