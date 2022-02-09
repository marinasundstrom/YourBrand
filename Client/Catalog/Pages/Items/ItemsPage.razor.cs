using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Catalog.Client;
using MudBlazor;

namespace Catalog.Pages.Items
{
    public partial class ItemsPage
    {
        MudTable<ItemDto> table;
        HubConnection hubConnection;
        Stream imageToUpload = null;
        
        protected override async Task OnInitializedAsync()
        {
            try
            {
                hubConnection = new HubConnectionBuilder().WithUrl($"{NavigationManager.BaseUri}api/hubs/items", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var results = await AccessTokenProvider.RequestAccessToken(new AccessTokenRequestOptions()
                        {Scopes = new[]{"myapi"}});
                        if (results.TryGetToken(out var accessToken))
                        {
                            return accessToken.Value;
                        }

                        return null !;
                    };
                }).WithAutomaticReconnect().Build();
                hubConnection.On<ItemDto>("ItemAdded", OnItemAdded);
                hubConnection.On<string, string>("ItemDeleted", OnItemDeleted);
                hubConnection.On<string, string>("ImageUploaded", OnImageUploaded);
                hubConnection.Closed += (error) =>
                {
                    if (error is not null)
                    {
                        Snackbar.Add($"{error.Message}", Severity.Error);
                    }

                    Snackbar.Add("Connection closed");
                    return Task.CompletedTask;
                };
                hubConnection.Reconnected += (error) =>
                {
                    Snackbar.Add("Reconnected");
                    return Task.CompletedTask;
                };
                hubConnection.Reconnecting += (error) =>
                {
                    Snackbar.Add("Reconnecting");
                    return Task.CompletedTask;
                };
                await hubConnection.StartAsync();
                Snackbar.Add("Connected");
            }
            catch (Exception exc)
            {
                Snackbar.Add(exc.Message.ToString(), Severity.Error);
            }
        }

        async Task OnItemAdded(ItemDto item)
        {
            Snackbar.Add("Item was added", Severity.Success);
            if (imageToUpload is not null)
            {
                try
                {
                    await ItemsClient.UploadImageAsync(item.Id, new FileParameter(imageToUpload));
                }
                catch (Exception exc)
                {
                    Snackbar.Add(exc.Message, Severity.Error);
                }
            }

            imageToUpload?.Dispose();
            imageToUpload = null;
            await table.ReloadServerData();
        }

        async Task OnItemDeleted(string id, string name)
        {
            Snackbar.Add($"\"{name}\" was removed", Severity.Success);
            await table.ReloadServerData();
        }

        async Task OnImageUploaded(string id, string image)
        {
            Snackbar.Add($"Image was uploaded", Severity.Success);
            await table.ReloadServerData();
        }

        private async Task<TableData<ItemDto>> ServerReload(TableState state)
        {
            try
            {
                var results = await ItemsClient.GetItemsAsync(state.Page, state.PageSize, state.SortLabel, state.SortDirection == MudBlazor.SortDirection.Ascending ? Catalog.Client.SortDirection.Asc : Catalog.Client.SortDirection.Desc);
                return new TableData<ItemDto>()
                {TotalItems = results.TotalCount, Items = results.Items};
            }
            catch (AccessTokenNotAvailableException exc)
            {
                return null !;
            }
        }

        private void RowClickEvent(TableRowClickEventArgs<ItemDto> args)
        {
            //NotificationService.ShowNotification("Message", $"You clicked {args.Item.Name}");
            NavigationManager.NavigateTo($"/items/{args.Item.Id}");
        }

        private async Task OpenDialog()
        {
            var dialogReference = DialogService.Show<AddItemDialog>("New item");
            var result = await dialogReference.Result;
            var model = (AddItemDialog.FormModel)result.Data;
            if (result.Cancelled)
                return;
            try
            {
                imageToUpload = model.Stream;
                await ItemsClient.AddItemAsync(new AddItemDto()
                {Name = model.Name, Description = model.Description});
            }
            catch (Exception exc)
            {
                Snackbar.Add(exc.Message.ToString(), Severity.Error);
            }
        }

        async Task DeleteItem(ItemDto item)
        {
            var result = await DialogService.ShowMessageBox($"Delete '{item.Name}'?", "Are you sure?", "Yes", "No");
            if (result.GetValueOrDefault())
            {
                await ItemsClient.DeleteItemAsync(item.Id);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }
    }
}