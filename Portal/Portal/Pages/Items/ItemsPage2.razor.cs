using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using YourBrand.AppService.Client;
using MudBlazor;

namespace YourBrand.Portal.Pages.Items
{
    public partial class ItemsPage2
    {
        int selectedPage;
        int pageCount;
        IEnumerable<ItemDto> items;

        HubConnection hubConnection;
        Stream imageToUpload = null;
        
        protected override async Task OnInitializedAsync()
        {
            await OnPageSelected(1);

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
            try
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
                await OnPageSelected(selectedPage);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        async Task OnItemDeleted(string id, string name)
        {
            Snackbar.Add($"\"{name}\" was removed", Severity.Success);
            await OnPageSelected(selectedPage);
        }

        async Task OnImageUploaded(string id, string image)
        {
            Snackbar.Add($"Image was uploaded", Severity.Success);
            await OnPageSelected(selectedPage);
        }

        private async Task OnPageSelected(int page)
        {
            int pageSize = 10;

            try
            {
                selectedPage = page;

                var results = await ItemsClient.GetItemsAsync(selectedPage - 1, pageSize, null, YourBrand.AppService.Client.SortDirection.Asc);

                items = results.Items;

                pageCount = (int)(results.TotalCount / 10);

                if(results.TotalCount % 10 != 0)
                {
                    pageCount++;
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }

            StateHasChanged();
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
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            catch (Exception exc)
            {
                Snackbar.Add(exc.Message.ToString(), Severity.Error);
            }
        }

        async Task DeleteItem(ItemDto item)
        {
            try
            {
                var result = await DialogService.ShowMessageBox($"Delete '{item.Name}'?", "Are you sure?", "Yes", "No");
                if (result.GetValueOrDefault())
                {
                    await ItemsClient.DeleteItemAsync(item.Id);
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }
    }
}