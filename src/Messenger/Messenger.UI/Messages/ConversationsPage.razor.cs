using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using MudBlazor;

using YourBrand.Messenger.Client;

namespace YourBrand.Messenger.Messages;

public partial class ConversationsPage
{
    MudTable<ConversationDto> table;

    private async Task<TableData<ConversationDto>> ServerReload(TableState state)
    {
        try
        {
            var results = await ConversationsClient.GetConversationsAsync(state.Page, state.PageSize, state.SortLabel, state.SortDirection == MudBlazor.SortDirection.Ascending ? global::YourBrand.Messenger.Client.SortDirection.Asc : global::YourBrand.Messenger.Client.SortDirection.Desc);
            return new TableData<ConversationDto>()
            { TotalItems = results.TotalCount, Items = results.Items };
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }

        return null!;
    }

    private void RowClickEvent(TableRowClickEventArgs<ConversationDto> args)
    {
        //NotificationService.ShowNotification("Message", $"You clicked {args.Item.Name}");
        NavigationManager.NavigateTo($"/conversations/{args.Item.Id}");
    }

    private async Task OpenDialog()
    {
        var dialogReference = DialogService.Show<NewConversationDialog>("New conversation");
        var result = await dialogReference.Result;
        var model = (NewConversationDialog.FormModel)result.Data;
        if (result.Cancelled)
            return;
        try
        {
            var dto = await ConversationsClient.CreateConversationAsync(model.Title);

            NavigationManager.NavigateTo($"Conversations/{dto.Id}");
        }
        catch (Exception exc)
        {
            Snackbar.Add(exc.Message.ToString(), Severity.Error);
        }
    }

    private async Task Join(ConversationDto conversation)
    {
        await ConversationsClient.JoinConversationAsync(conversation.Id);

        NavigationManager.NavigateTo($"Conversations/{conversation.Id}");
    }
}