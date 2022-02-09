using Microsoft.AspNetCore.Components;
using Catalog.Client;
using MudBlazor;
using Microsoft.AspNetCore.Components.Routing;

namespace Catalog.Pages.Items
{
    public partial class ItemPage
    {
        MudTable<CommentDto> table;
        ItemDto? item;

        [Parameter]
        public string Id { get; set; } = null !;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        async Task LoadAsync()
        {
            try
            {
                item = await ItemsClient.GetItemAsync(Id);
            }
            catch (ApiException<ProblemDetails> exc)
            {
                Snackbar.Add(exc.Result.Detail, Severity.Error);
            }
            catch (Exception exc)
            {
                Snackbar.Add(exc.Message, Severity.Error);
            }
        }

        async void OnLocationChanged(object sender, LocationChangedEventArgs ev)
        {
            await LoadAsync();
            await table.ReloadServerData();
            StateHasChanged();
        }

        private async Task<TableData<CommentDto>> ServerReload(TableState state)
        {
            var results = await ItemsClient.GetCommentsAsync(Id, state.Page, state.PageSize, state.SortLabel, state.SortDirection == MudBlazor.SortDirection.Ascending ? Catalog.Client.SortDirection.Asc : Catalog.Client.SortDirection.Desc);
            return new TableData<CommentDto>()
            {TotalItems = results.TotalCount, Items = results.Items};
        }

        private async Task OpenDialog()
        {
            var dialogReference = DialogService.Show<CreateCommentDialog>("Write a comment");
            var result = await dialogReference.Result;
            var model = (CreateCommentDialog.FormModel)result.Data;
            if (result.Cancelled)
                return;
            try
            {
                await ItemsClient.PostCommentAsync(Id, new PostCommentDto()
                {Text = model.Text});
                await table.ReloadServerData();
            }
            catch (Exception exc)
            {
                Snackbar.Add(exc.Message.ToString(), Severity.Error);
            }
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}