using Microsoft.AspNetCore.Components;
using Skynet.AppService.Client;
using MudBlazor;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Skynet.Portal.Pages.Items
{
    public partial class ItemPage
    {
        MudTable<CommentDto> table;
        ItemDto? item;
        bool loadingFailed = false;

        [Parameter]
        public string Id { get; set; } = null !;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        async Task LoadAsync()
        {
            loadingFailed = false;

            #if DEBUG
            await Task.Delay(2000);
            #endif

            try
            {
                //throw new Exception();

                item = await ItemsClient.GetItemAsync(Id);
            }
            catch (ApiException<ProblemDetails> exc)
            {
                loadingFailed = true;

                Snackbar.Add(exc.Result.Detail, Severity.Error);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            catch (Exception exc)
            {
                loadingFailed = true;

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
            try
            { 
                var results = await ItemsClient.GetCommentsAsync(Id, state.Page, state.PageSize, state.SortLabel, state.SortDirection == MudBlazor.SortDirection.Ascending ? Skynet.AppService.Client.SortDirection.Asc : Skynet.AppService.Client.SortDirection.Desc);
                return new TableData<CommentDto>()
                {
                    TotalItems = results.TotalCount,
                    Items = results.Items
                };

            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }

            return null!;
        }

        private async Task OpenDialog(CommentDto? comment)
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(CommentDialog.ItemId), Id);
            parameters.Add(nameof(CommentDialog.CommentId), comment?.Id);

            var dialogReference = DialogService.Show<CommentDialog>(comment is not null ? "Update comment" : "Write a comment", parameters);
            var result = await dialogReference.Result;
            var model = (CommentDialog.FormModel)result.Data;

            if (result.Cancelled)
                return;

            await table.ReloadServerData();
        }

        private async Task DeleteComment(CommentDto comment)
        {
            var result = await DialogService.ShowMessageBox($"Delete comment?", "Are you sure?", "Yes", "No");
            if (result.GetValueOrDefault())
            {
                await ItemsClient.DeleteCommentAsync(Id, comment.Id);

                await table.ReloadServerData();
            }
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}