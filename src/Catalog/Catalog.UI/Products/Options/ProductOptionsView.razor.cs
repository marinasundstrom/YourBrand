using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace YourBrand.Catalog.Products.Options;

sealed partial class ProductOptionsView : ComponentBase
{
    MudTable<ProductOption> productOptionsTable = default!;

    readonly TableGroupDefinition<ProductOption> tableGroupDefinition = new TableGroupDefinition<ProductOption>()
    {
        GroupName = "Group",
        Indentation = false,
        Expandable = true,
        Selector = (e) => e.Option.Group?.Name
    };

    ProductOption? selectedProductOption;

    string? searchString;

    [Parameter]
    [EditorRequired]
    public long ProductId { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public IReadOnlyCollection<ProductOption> ProductOptions { get; set; } = default!;

    [CascadingParameter(Name = "Organization")]
    public YourBrand.Portal.Services.Organization Organization { get; set; }

    private bool FilterOptionsFunc(ProductOption productOption)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (productOption.Option.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
            return true;

        return false;
    }

    private async Task<IEnumerable<OptionValue>> SearchOptionValue(string value, CancellationToken cancellationToken)
    {
        /*
        var option = await OptionsClient.GetOptionsAsync(selectedProductOption!.Id, cancellationToken);

        if (value is null)
            return option.Values;

        return option.Values.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        */

        return Enumerable.Empty<OptionValue>();
    }

    private async Task OnSearch(string text)
    {
        searchString = text;
        await productOptionsTable.ReloadServerData();
    }

    async Task ShowAddOptionDialog()
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(CreateProductOptionModal.ProductId), ProductId);

        var dialogRef = await DialogService.ShowAsync<CreateProductOptionModal>("Create option", parameters);

        var dialogResult = await dialogRef.Result;

        if (dialogResult.Canceled) return;

        ProductOptions = (await ProductOptionsClient.GetProductOptionsAsync(Organization.Id, ProductId, null)).ToList();
    }

    async Task ShowEditOptionDialog(ProductOption option)
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(UpdateProductOptionModal.ProductId), ProductId);
        parameters.Add(nameof(UpdateProductOptionModal.Option), option.Option);

        var dialogRef = await DialogService.ShowAsync<UpdateProductOptionModal>("Edit option", parameters);

        var dialogResult = await dialogRef.Result;

        if (dialogResult.Canceled) return;

        ProductOptions = (await ProductOptionsClient.GetProductOptionsAsync(Organization.Id, ProductId, null)).ToList();
    }

    async Task ShowOptionGroupsDialog()
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(ProductOptionGroupsDialog.ProductId), ProductId);

        var dialogRef = await DialogService.ShowAsync<ProductOptionGroupsDialog>("Option groups", parameters);

        var dialogResult = await dialogRef.Result;

        ProductOptions = (await ProductOptionsClient.GetProductOptionsAsync(Organization.Id, ProductId, null)).ToList();
    }

    async Task ShowEditOptionGroupDialog(OptionGroup optionGroup)
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(UpdateProductOptionGroupModal.ProductId), ProductId);
        parameters.Add(nameof(UpdateProductOptionGroupModal.OptionGroup), optionGroup);

        var dialogRef = await DialogService.ShowAsync<CreateProductOptionGroupModal>("Edit option group", parameters);

        var dialogResult = await dialogRef.Result;

        if (dialogResult.Canceled) return;

        ProductOptions = (await ProductOptionsClient.GetProductOptionsAsync(Organization.Id, ProductId, null)).ToList();
    }

    async Task DeleteOption(string optionId)
    {
        var result = await DialogService.ShowMessageBox("Delete option", "Are you sure?", "Yes", "No");

        if (!result.GetValueOrDefault()) return;

        await ProductOptionsClient.DeleteProductOptionAsync(Organization.Id, ProductId, optionId);

        ProductOptions = (await ProductOptionsClient.GetProductOptionsAsync(Organization.Id, ProductId, null)).ToList();
    }
}