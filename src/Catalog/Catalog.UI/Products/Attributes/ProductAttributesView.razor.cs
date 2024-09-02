using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace YourBrand.Catalog.Products.Attributes;

sealed partial class ProductAttributesView : ComponentBase
{
    readonly MudTable<ProductAttribute> productAttributesTable = default!;
    readonly TableGroupDefinition<ProductAttribute> tableGroupDefinition = new TableGroupDefinition<ProductAttribute>()
    {
        GroupName = "Group",
        Indentation = false,
        Expandable = true,
        Selector = (e) => e.Attribute.Group?.Name
    };

    readonly ProductAttribute? selectedProductAttribute;
    ProductAttribute? productAttributeBeforeEdit;

    string? searchString;

    [Parameter]
    [EditorRequired]
    public long ProductId { get; set; } = default!;

    [Parameter]
    public bool HasVariants { get; set; } = false!;

    public async Task ReloadAsync()
    {
        await productAttributesTable.ReloadServerData();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<ProductAttribute>> ServerReload(TableState state)
    {
        var result = await ProductsClient.GetProductAttributesAsync(ProductId); /*, state.Page + 1, state.PageSize,
        searchString,
        state.SortLabel, state.SortDirection == MudBlazor.SortDirection.None ? null : (state.SortDirection ==
        MudBlazor.SortDirection.Descending ? YourBrand.Catalog.SortDirection.Desc : YourBrand.Catalog.SortDirection.Asc)); */

        return new TableData<ProductAttribute>() { TotalItems = result.Count, Items = result };
    }

    private async Task OnSearch(string text)
    {
        searchString = text;
        await productAttributesTable.ReloadServerData();
    }

    private async Task DeleteProductAttribute(ProductAttribute args)
    {
        //NavigationManager.NavigateTo($"/products/{args.Item.Id}");

        await ProductsClient.DeleteProductAttributeAsync(ProductId, args.Attribute.Id);

        await productAttributesTable.ReloadServerData();
    }

    private void BackupItem(object productAttribute)
    {
        productAttributeBeforeEdit = new()
        {
            Attribute = ((ProductAttribute)productAttribute).Attribute,
            Value = ((ProductAttribute)productAttribute).Value,
            ForVariant = ((ProductAttribute)productAttribute).ForVariant,
            IsMainAttribute = ((ProductAttribute)productAttribute).IsMainAttribute
        };
    }

    private async void ItemHasBeenCommitted(object productAttribute)
    {
        if (productAttribute is ProductAttribute pa)
        {
            await ProductsClient.UpdateProductAttributeAsync(ProductId, pa.Attribute.Id, new UpdateProductAttribute
            {
                ValueId = pa.Value.Id,
                ForVariant = pa.ForVariant,
                IsMainAttribute = pa.IsMainAttribute
            });
        }
    }

    private void ResetItemToOriginalValues(object productAttribute)
    {
        ((ProductAttribute)productAttribute).Attribute = productAttributeBeforeEdit!.Attribute;
        ((ProductAttribute)productAttribute).Value = productAttributeBeforeEdit.Value;
        ((ProductAttribute)productAttribute).ForVariant = productAttributeBeforeEdit!.ForVariant;
        ((ProductAttribute)productAttribute).IsMainAttribute = productAttributeBeforeEdit.IsMainAttribute;
    }

    private bool FilterAttributesFunc(ProductAttribute productAttribute)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (productAttribute.Attribute.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
            return true;

        return false;
    }

    private async Task<IEnumerable<AttributeValue>> SearchAttributeValue(string value, CancellationToken cancellationToken)
    {
        var attribute = await AttributesClient.GetAttributeByIdAsync(selectedProductAttribute!.Attribute.Id, cancellationToken);

        if (value is null)
            return attribute.Values;

        return attribute.Values.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task OpenAddProductAttributeDialog()
    {
        var param = new DialogParameters();
        param.Add(nameof(AddProductAttributeDialog.ProductId), ProductId);
        var dialog = await DialogService.ShowAsync<AddProductAttributeDialog>("Add product attribute", param);
        var result = await dialog.Result;
        if (result.Canceled)
        {
            return;
        }

        await productAttributesTable.ReloadServerData();
    }
}