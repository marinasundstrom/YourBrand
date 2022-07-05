namespace YourBrand.Products.Application;

using System;

using Azure.Storage.Blobs;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Application.Options;
using YourBrand.Products.Application.Products;
using YourBrand.Products.Application.Products.Attributes;
using YourBrand.Products.Application.Products.Groups;
using YourBrand.Products.Application.Products.Options;
using YourBrand.Products.Application.Products.Options.Groups;
using YourBrand.Products.Application.Products.Variants;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

public class Api
{
    private readonly IMediator _mediator;

    public Api(IMediator mediator) 
    {
        _mediator = mediator;
    }

    public async Task<ApiProductsResult> GetProducts(bool includeUnlisted = false, string? groupId = null, int page = 10, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetProducts(includeUnlisted, groupId, page, pageSize), cancellationToken);
    }

    public async Task<ApiProduct?> GetProduct(string productId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetProduct(productId), cancellationToken);
    }

    public async Task<string?> UploadProductImage(string productId, string fileName, Stream stream, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new UploadProductImage(productId, fileName, stream), cancellationToken); 
    }

    public async Task<string?> UploadProductVariantImage(string productId, string variantId, string fileName, Stream stream, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new UploadProductVariantImage(productId, variantId, fileName, stream), cancellationToken); 
    }

    public async Task<ApiProduct?> CreateProduct(ApiCreateProduct data, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateProduct(data.Name, data.HasVariants, data.Description, data.GroupId, data.SKU, data.Price, data.Visibility), cancellationToken);
    }

    public async Task UpdateProductDetails(string productId, ApiUpdateProductDetails data, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateProductDetails(productId, data), cancellationToken);
    }

    public async Task UpdateProductVisibility(string productId, ProductVisibility visibility, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateProductVisibility(productId, visibility), cancellationToken);
    }

    public async Task<ApiOptionValue> CreateProductOptionValue(string productId, string optionId, ApiCreateProductOptionValue data, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateProductOptionValue(productId, optionId, data), cancellationToken);
    }

    public async Task<ApiOption> CreateProductOption(string productId, ApiCreateProductOption data, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateProductOption(productId, data), cancellationToken);
    }

    public async Task DeleteProductOption(string productId, string optionId)
    {
        await _mediator.Send(new DeleteProductOption(productId, optionId));
    }

    public async Task<ApiOption> UpdateProductOption(string productId, string optionId, ApiUpdateProductOption data, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new UpdateProductOption(productId, optionId, data), cancellationToken);
    }

    public async Task<IEnumerable<ApiOption>> GetProductOptions(string productId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetProductOptions(productId), cancellationToken);
    }

    public async Task<IEnumerable<ApiAttribute>> GetProductAttributes(string productId)
    {
        return await _mediator.Send(new GetProductAttributes(productId));
    }

    public async Task DeleteProductOptionValue(string productId, string optionId, string valueId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductOptionValue(productId, optionId, valueId), cancellationToken);
    }

    public async Task<IEnumerable<ApiOption>> GetOptions(bool includeChoices)
    {
        return await _mediator.Send(new GetOptions(includeChoices));
    }

    public async Task<IEnumerable<ApiAttribute>> GetAttributes()
    {
        return await _mediator.Send(new GetAttributes());
    }

    public async Task<ApiOption> GetOption(string optionId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetOption(optionId), cancellationToken);
    }

    public async Task<ApiAttribute> GetAttribute(string attributeId)
    {
        return await _mediator.Send(new GetAttribute(attributeId));
    }

    public async Task<IEnumerable<ApiProductGroup>> GetProductGroups(bool includeWithUnlistedProducts = false)
    {
        return await _mediator.Send(new GetProductGroups(includeWithUnlistedProducts));
    }


    public async Task<ApiProductGroup?> CreateProductGroup(ApiCreateProductGroup data)
    {
        return await _mediator.Send(new CreateProductGroup(data.Name, data));
    }

    public async Task<ApiProductGroup?> UpdateProductGroup(string productGroupId, ApiUpdateProductGroup data)
    {
        return await _mediator.Send(new UpdateProductGroup(productGroupId, data));
    }

    public async Task DeleteProductGroup(string productGroupId)
    {
        await _mediator.Send(new DeleteProductGroup(productGroupId));
    }

    public async Task<IEnumerable<ApiOptionGroup>> GetOptionGroups(string productId)
    {
        return await _mediator.Send(new GetProductOptionGroups(productId));
    }

    public async Task<ApiOptionGroup?> CreateOptionGroup(string productId, ApiCreateProductOptionGroup data)
    {
        return await _mediator.Send(new CreateProductOptionGroup(productId, data));
    }

    public async Task<ApiOptionGroup?> UpdateOptionGroup(string productId, string optionGroupId, ApiUpdateProductOptionGroup data)
    {
        return await _mediator.Send(new UpdateProductOptionGroup(productId, optionGroupId, data));
    }

    public async Task DeleteOptionGroup(string productId, string optionGroupId)
    {
        await _mediator.Send(new DeleteProductOptionGroup(productId, optionGroupId));
    }

    public async Task<ApiProductVariant> CreateVariant(string productId, ApiCreateProductVariant data)
    {
        return await _mediator.Send(new CreateProductVariant(productId, data));
    }

    private static string?  GetImageUrl(string? name)
    {
        return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
    }

    public async Task DeleteVariant(string productId, string productVariantId)
    {
        await _mediator.Send(new DeleteProductVariant(productId, productVariantId));
    }

    public async Task<ApiProductVariant> UpdateVariant(string productId, string productVariantId, ApiUpdateProductVariant data)
    {
        return await _mediator.Send(new UpdateProductVariant(productId, productVariantId, data));
    }

    public async Task<IEnumerable<ApiOptionValue>> GetOptionValues(string optionId)
    {
        return await _mediator.Send(new GetOptionValues(optionId));
    }

    public async Task<IEnumerable<ApiAttributeValue>> GetAttributeValues(string attributeId)
    {
        return await _mediator.Send(new GetAttributeValues(attributeId));
    }

    public async Task<IEnumerable<ApiProductVariant>> GetProductVariants(string productId)
    {
        return await _mediator.Send(new GetProductVariants(productId));
    }

    public async Task<ApiProductVariant?> GetProductVariant(string productId, string productVariantId)
    {
        return await _mediator.Send(new GetProductVariant(productId, productVariantId));
    }

    public async Task<ApiProductVariant?> FindProductVariant(string productId, Dictionary<string, string?> selectedOptions)
    {
        return await _mediator.Send(new FindProductVariant(productId, selectedOptions));
    }

    public async Task<IEnumerable<ApiProductVariantOption>> GetProductVariantOptions(string productId, string productVariantId)
    {
        return await _mediator.Send(new GetProductVariantOptions(productId, productVariantId));
    }

    public async Task<IEnumerable<ApiOptionValue>> GetAvailableOptionValues(string productId, string optionId, IDictionary<string, string?> selectedOptions)
    {
        return await _mediator.Send(new GetAvailableOptionValues(productId, optionId, selectedOptions));
    }
}

public record class ApiProductsResult(IEnumerable<ApiProduct> Items, int Total);

public record class ApiProduct(string Id, string Name, string? Description, ApiProductGroup? Group, string? SKU, string? Image, decimal? Price, bool HasVariants, ProductVisibility? Visibility);

public record class ApiCreateProduct(string Name, bool HasVariants, string? Description, string? GroupId, string? SKU, decimal? Price, ProductVisibility? Visibility);

public record class ApiUpdateProductDetails(string Name, string? Description, string? SKU, string? Image, decimal? Price, string? GroupId);


public record class ApiProductGroup(string Id, string Name, string? Description, string? ParentGroupId);

public record class ApiCreateProductGroup(string Name, string? Description, string? ParentGroupId);

public record class ApiUpdateProductGroup(string Name, string? Description, string? ParentGroupId);


public record class ApiOption(string Id, string Name, string? Description, OptionType OptionType, ApiOptionGroup? Group, string? SKU, decimal? Price, bool IsSelected, IEnumerable<ApiOptionValue> Values, ApiOptionValue? DefaultValue);

public record class ApiAttribute(string Id, string Name, string? Description, ApiAttributeGroup? Group, IEnumerable<ApiAttributeValue> Values);

public record class ApiOptionValue(string Id, string Name, string? SKU, decimal? Price, int? Seq);

public record class ApiAttributeValue(string Id, string Name, int? Seq);

public record class ApiCreateProductOption(string Name, string? Description, OptionType OptionType, ApiOptionGroup? Group, string? SKU, decimal? Price, string? GroupId, IEnumerable<ApiCreateProductOptionValue> Values);

public record class ApiCreateProductOptionValue(string Name, string? SKU, decimal? Price);

public record class ApiUpdateProductOption(string Name, string? Description, OptionType OptionType, string? SKU, decimal? Price, string? GroupId, IEnumerable<ApiUpdateProductOptionValue> Values);

public record class ApiUpdateProductOptionValue(string? Id, string Name, string? SKU, decimal? Price);


public record class ApiOptionGroup(string Id, string Name, string? Description, int? Seq, int? Min, int? Max);

public record class ApiAttributeGroup(string Id, string Name, string? Description);

public record class ApiCreateProductOptionGroup(string Name, string? Description, int? Min, int? Max);

public record class ApiUpdateProductOptionGroup(string Name, string? Description, int? Min, int? Max);


public record class ApiProductVariant(string Id, string Name, string? Description, string? SKU, string? Image, decimal? Price, IEnumerable<ApiProductVariantOption> Options);

public record class ApiProductVariantOption(string Id, string Name, string Value);


public record class ApiCreateProductVariant(string Name, string? Description, string SKU, decimal Price, IEnumerable<ApiCreateProductVariantOption> Values);

public record class ApiCreateProductVariantOption(string OptionId, string ValueId);


public record class ApiUpdateProductVariant(string Name, string? Description, string SKU, decimal Price, IEnumerable<ApiUpdateProductVariantOption> Options);

public record class ApiUpdateProductVariantOption(int? Id, string OptionId, string ValueId);


public class VariantAlreadyExistsException : Exception
{
    public VariantAlreadyExistsException(string message) : base(message) { }
}


public enum OptionType { Single, Multiple }

public enum ProductVisibility { Unlisted, Listed }

