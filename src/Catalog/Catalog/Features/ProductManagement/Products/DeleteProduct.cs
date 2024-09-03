using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record DeleteProduct(string OrganizationId, string IdOrHandle) : IRequest<Result>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<DeleteProduct, Result>
    {
        public async Task<Result> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products
                .InOrganization(request.OrganizationId)
                .IncludeBasics()
                .FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products
                .InOrganization(request.OrganizationId).IncludeBasics()
                .FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            using var transaction = await catalogContext.Database.BeginTransactionAsync();

            await catalogContext.SaveChangesAsync(cancellationToken);

            product.Category?.RemoveProduct(product);

            await catalogContext.ProductAttributes
                .InOrganization(request.OrganizationId)
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.AttributeGroups
                .InOrganization(request.OrganizationId)
                .Where(x => x.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ProductOptions
                .InOrganization(request.OrganizationId)
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.Products
                .InOrganization(request.OrganizationId)
                .Where(x => x.ParentId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ProductVariantOptions
                .InOrganization(request.OrganizationId)
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ChoiceOptions
                .InOrganization(request.OrganizationId)
                .Where(x => x.Group!.Product!.Id == product.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(z => z.DefaultValueId, (string?)null), cancellationToken);

            await catalogContext.OptionValues
                .InOrganization(request.OrganizationId)
                .Where(x => x.Option.Group!.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.Options
                .InOrganization(request.OrganizationId)
                .Where(x => x.Group!.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.OptionGroups
                .InOrganization(request.OrganizationId)
                .Where(x => x.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await catalogContext.ProductImages
                .InOrganization(request.OrganizationId)
                .Where(x => x.Product!.Id == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            catalogContext.Products.Remove(product);

            await catalogContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync();

            return Result.Success();
        }
    }
}