namespace YourBrand.Products.Application;

using System;

using Azure.Storage.Blobs;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Application.Common.Models;
using YourBrand.Products.Application.Options;
using YourBrand.Products.Application.Products;
using YourBrand.Products.Application.Products.Attributes;
using YourBrand.Products.Application.Products.Groups;
using YourBrand.Products.Application.Products.Options;
using YourBrand.Products.Application.Products.Options.Groups;
using YourBrand.Products.Application.Products.Variants;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

public record class ApiCreateProduct(string Name, bool HasVariants, string? Description, string? GroupId, string? SKU, decimal? Price, ProductVisibility? Visibility);

public record class ApiUpdateProductDetails(string Name, string? Description, string? SKU, string? Image, decimal? Price, string? GroupId);

public record class ApiCreateProductGroup(string Name, string? Description, string? ParentGroupId);

public record class ApiUpdateProductGroup(string Name, string? Description, string? ParentGroupId);

public record class ApiCreateProductOption(string Name, string? Description, OptionType OptionType, OptionGroupDto? Group, string? SKU, decimal? Price, string? GroupId, IEnumerable<ApiCreateProductOptionValue> Values);

public record class ApiCreateProductAttribute(string Name, string? Description, string? GroupId, IEnumerable<ApiCreateProductOptionValue> Values);


public record class ApiCreateProductOptionValue(string Name, string? SKU, decimal? Price);

public record class ApiCreateProductAttributeValue(string Name);

public record class ApiUpdateProductOption(string Name, string? Description, OptionType OptionType, string? SKU, decimal? Price, string? GroupId, IEnumerable<ApiUpdateProductOptionValue> Values);

public record class ApiUpdateProductAttribute(string Name, string? Description, string GroupId, IEnumerable<ApiUpdateProductOptionValue> Values);

public record class ApiUpdateProductOptionValue(string? Id, string Name, string? SKU, decimal? Price);

public record class ApiUpdateProductAttributeValue(string? Id, string Name);


public record class ApiCreateProductOptionGroup(string Name, string? Description, int? Min, int? Max);

public record class ApiUpdateProductOptionGroup(string Name, string? Description, int? Min, int? Max);

public record class ProductVariantDtoOption(string Id, string Name, string Value);


public record class ApiCreateProductAttributeGroup(string Name, string? Description);

public record class ApiUpdateProductAttributeGroup(string Name, string? Description);



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

