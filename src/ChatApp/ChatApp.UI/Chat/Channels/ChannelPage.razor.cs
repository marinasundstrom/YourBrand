using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR.Client;

using MudBlazor;

using YourBrand.ChatApp.Features.Chat;
using YourBrand.Portal.Theming;

namespace YourBrand.ChatApp.Chat.Channels;

public partial class ChannelPage : IChatHubClient
{
    bool isDarkMode = false;
    string currentUserId = "BS";
    bool isInAdminRole = false;
    string? editingMessageId = null;
    MessageViewModel? replyToMessage = null;

    readonly List<MessageViewModel> messagesCache = new List<MessageViewModel>();
    readonly List<MessageViewModel> loadedMessages = new List<MessageViewModel>();

    bool loaded = false;

    [Parameter]
    public string? Id { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    HubConnection hubConnection = null!;
    IChatHub hubProxy = default!;

    UserInfo userInfo = default!;

    protected override async Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        ThemeManager.ColorSchemeChanged += ColorSchemeChanged;
        isDarkMode = ThemeManager.CurrentColorScheme == ColorScheme.Dark;

        StateHasChanged();

        currentUserId = await UserContext.GetUserId()!;
        isInAdminRole = await UserContext.IsUserInRole("admin");

        userInfo = await UsersClient.GetUserInfoAsync();

        await LoadChannel();
    }

    private async Task LoadChannel()
    {
        Id = Id ?? "73b202c5-3ef1-4cd8-b1ed-04c05f47e981";

        await LoadMessages();

        loaded = true;

        StateHasChanged();

        await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");

        try
        {
            if (hubConnection is not null && hubConnection.State != HubConnectionState.Disconnected)
            {
                await hubConnection.DisposeAsync();
            }

            hubConnection = new HubConnectionBuilder().WithUrl($"{NavigationManager.BaseUri}api/messenger/hubs/chat?organizationId={Organization.Id}&channelId={Id}", options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    return await AccessTokenProvider.GetAccessTokenAsync();
                };
            }).WithAutomaticReconnect().Build();


            hubProxy = hubConnection.ServerProxy<IChatHub>();
            _ = hubConnection.ClientRegistration<IChatHubClient>(this);

            hubConnection.Closed += (error) =>
            {
                if (error is not null)
                {
                    Snackbar.Add($"{error.Message}", Severity.Error, configure: options =>
                    {
                        options.Icon = Icons.Material.Filled.Cable;
                    });
                }

                Snackbar.Add(T["Disconnected"], configure: options =>
                {
                    options.Icon = Icons.Material.Filled.Cable;
                });

                return Task.CompletedTask;
            };

            hubConnection.Reconnected += (error) =>
            {
                Snackbar.Add(T["Reconnected"], configure: options =>
                {
                    options.Icon = Icons.Material.Filled.Cable;
                });

                return Task.CompletedTask;
            };

            hubConnection.Reconnecting += (error) =>
            {
                Snackbar.Add(T["Reconnecting"], configure: options =>
                {
                    options.Icon = Icons.Material.Filled.Cable;
                });

                return Task.CompletedTask;
            };

            await hubConnection.StartAsync();

            /*
            Snackbar.Add(T["Connected"], configure: options => {
                options.Icon = Icons.Material.Filled.Cable;
            });
            */
        }
        catch (Exception exc)
        {
            Snackbar.Add(exc.Message.ToString(), Severity.Error);
        }
    }

    private async Task LoadMessages()
    {
        var result = await MessagesClient.GetMessagesAsync(Organization.Id, Id, 1, 10, null, null);

        loadedMessages.Clear();

        foreach (var item in result.Items.Reverse())
        {
            AddOrUpdateMessage(item);
        }
    }

    private void AddOrUpdateMessage(ChatApp.Message message)
    {
        var messageVm = loadedMessages.FirstOrDefault(x => x.Id == message.Id);

        if (messageVm is not null)
        {
            messageVm.Posted = message.Posted;
            messageVm.Content = message.Content;
            messageVm.LastEdited = message.LastEdited;
            messageVm.LastEditedById = message.LastEditedBy?.Id;
            messageVm.LastEditedByName = message.LastEditedBy?.Name;
            messageVm.Deleted = message.Deleted;
            messageVm.DeletedById = message.DeletedBy?.Id;
            messageVm.DeletedByName = message.DeletedBy?.Name;
            messageVm.IsFromCurrentUser = message.PostedBy.UserId == currentUserId;

            messageVm.Reactions = message.Reactions.ToList();

            // This is a new incoming message:

            if (message.PostedBy.UserId != currentUserId)
            {
                messageVm.Id = message.Id;
                messageVm.PostedById = message.PostedBy.Id;
                messageVm.PostedByUserId = message.PostedBy.UserId;
                messageVm.PostedByName = message.PostedBy.Name;
                messageVm.PostedByInitials = GetInitials(message.PostedBy.Name);
                messageVm.Content = message.Content;
                messageVm.ReplyTo = message.ReplyTo is null ? null : GetOrCreateReplyMessageVm(message.ReplyTo);
                messageVm.Confirmed = true;
            }

            return;
        }

        messageVm = new MessageViewModel
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            PostedById = message.PostedBy.Id,
            PostedByUserId = message.PostedBy.UserId,
            PostedByName = message.PostedBy.Name,
            PostedByInitials = GetInitials(message.PostedBy.Name),
            Posted = message.Posted,
            LastEdited = message.LastEdited,
            LastEditedById = message.LastEditedBy?.Id,
            LastEditedByName = message.LastEditedBy?.Name,
            Content = message.Content,
            IsFromCurrentUser = message.PostedBy.UserId == currentUserId,
            ReplyTo = message.ReplyTo is null ? null : GetOrCreateReplyMessageVm(message.ReplyTo),
            Deleted = message.Deleted,
            DeletedById = message.DeletedBy?.Id,
            DeletedByName = message.DeletedBy?.Name,
            Confirmed = true,
            Reactions = message.Reactions.ToList()
        };

        loadedMessages.Add(messageVm);
        messagesCache.Add(messageVm);
    }

    private MessageViewModel? GetOrCreateReplyMessageVm(ReplyMessage replyMessage)
    {
        var existingMessageVm = messagesCache.FirstOrDefault(x => x.Id == replyMessage.Id);

        if (existingMessageVm is not null)
        {
            return existingMessageVm;
        }

        var messageVm = new MessageViewModel
        {
            Id = replyMessage.Id,
            Content = replyMessage.Content,
            Posted = replyMessage.Posted,
            Deleted = replyMessage.Deleted,
            //IsFromCurrentUser = replyMessage.PostedBy.Id == currentUserId,
            Confirmed = true
        };

        messagesCache.Add(messageVm);

        return messageVm;
    }

    private async Task NotifyParticipants(Message message)
    {
        if (message.ReplyTo is null)
        {
            if (message.PostedBy.UserId == currentUserId)
            {
                await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
            }
            else
            {
                // TODO: Only display when outside viewport

                Snackbar.Add($"{message.PostedBy.Name} said: \"{message.Content}\"", Severity.Normal, options =>
                {
                    options.OnClick = async (sb) =>
                    {
                        await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
                    };
                });
            }
        }
    }

    async void OnLocationChanged(object? sender, LocationChangedEventArgs eventArgs)
    {
        ResetChannelWindow();

        await LoadChannel();

        StateHasChanged();
    }

    private void ResetChannelWindow()
    {
        Text = string.Empty;
        replyToMessage = null;
        editingMessageId = null;
    }

    void ColorSchemeChanged(object? sender, ColorSchemeChangedEventArgs arg)
    {
        isDarkMode = arg.ColorScheme == ColorScheme.Dark;
        StateHasChanged();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
        ThemeManager.ColorSchemeChanged -= ColorSchemeChanged;
    }

    /*
    [Required(
        ErrorMessageResourceName = "Required", 
        ErrorMessageResourceType = typeof(ChannelPage))] */
    public string Text { get; set; } = default!;

    async Task Send()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        if (editingMessageId is not null)
        {
            await UpdateMessage();

            return;
        }

        var message = new MessageViewModel()
        {
            Id = string.Empty,
            Posted = DateTimeOffset.UtcNow,
            PostedById = "", // Todo fix
            PostedByUserId = currentUserId,
            PostedByName = userInfo.Name,
            PostedByInitials = GetInitials(userInfo.Name), // TODO: Fix with my name,
            ReplyTo = replyToMessage,
            IsFromCurrentUser = true,
            Content = Text
        };

        loadedMessages.Add(message);
        messagesCache.Add(message);

        message.Id = await hubConnection.InvokeAsync<string>("PostMessage", Id, replyToMessage?.Id, Text);

        Text = string.Empty;
        replyToMessage = null;

        await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
    }

    private async Task UpdateMessage()
    {
        var messageVm = loadedMessages.FirstOrDefault(x => x.Id == editingMessageId);

        if (messageVm is not null)
        {
            messageVm.Content = Text;

            await MessagesClient.EditMessageAsync(Organization.Id, Id, editingMessageId, Text);

            Text = string.Empty;
            editingMessageId = null;
        }
    }

    async Task DeleteMessage(MessageViewModel messageVm)
    {
        await MessagesClient.DeleteMessageAsync(Organization.Id, Id, messageVm.Id);

        if (messageVm is not null)
        {
            if (replyToMessage?.Id == messageVm.Id)
            {
                AbortReplyToMessage();
            }

            if (editingMessageId == messageVm.Id)
            {
                AbortEditMessage();
            }
        }

        StateHasChanged();
    }

    void EditMessage(MessageViewModel messageVm)
    {
        replyToMessage = null;
        editingMessageId = messageVm.Id;
        Text = messageVm.Content;

        StateHasChanged();
    }

    void AbortEditMessage()
    {
        editingMessageId = null;
        Text = string.Empty;
    }

    async Task ReplyToMessage(MessageViewModel messageVm)
    {
        editingMessageId = null;
        replyToMessage = messageVm;
        Text = string.Empty;

        StateHasChanged();

        await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
    }

    void AbortReplyToMessage()
    {
        replyToMessage = null;
        Text = string.Empty;
    }

    async Task React(MessageViewModel messageVm, string reaction)
    {
        await MessagesClient.ReactAsync(Organization.Id, Id, messageVm.Id, reaction);

        await InvokeAsync(StateHasChanged);
    }

    private bool IsFirst(MessageViewModel currentMessage)
    {
        var index = loadedMessages.IndexOf(currentMessage);
        if (index == 0)
        {
            return true;
        }

        var previousMessage = loadedMessages[index - 1];

        if (previousMessage.PostedById != currentMessage.PostedById)
        {
            return true;
        }

        if (!(currentMessage.Posted.Year == previousMessage.Posted.Year && currentMessage.Posted.Month == previousMessage.Posted.Month && currentMessage.Posted.Day == previousMessage.Posted.Day && currentMessage.Posted.Hour == previousMessage.Posted.Hour && currentMessage.Posted.Minute == previousMessage.Posted.Minute))
        {
            return true;
        }

        return previousMessage.PostedById != currentMessage.PostedById;
    }

    private bool IsLast(MessageViewModel currentMessage)
    {
        var index = loadedMessages.IndexOf(currentMessage);
        if (index == loadedMessages.Count - 1)
        {
            return true;
        }

        var nextMessage = loadedMessages[index + 1];

        if (nextMessage.PostedById != currentMessage.PostedById)
        {
            return true;
        }

        if (!(currentMessage.Posted.Year == nextMessage.Posted.Year && currentMessage.Posted.Month == nextMessage.Posted.Month && currentMessage.Posted.Day == nextMessage.Posted.Day && currentMessage.Posted.Hour == nextMessage.Posted.Hour && currentMessage.Posted.Minute == nextMessage.Posted.Minute))
        {
            return true;
        }

        return nextMessage.PostedById != currentMessage.PostedById;
    }

    static string GetInitials(string name)
    {
        // StringSplitOptions.RemoveEmptyEntries excludes empty spaces returned by the Split method

        string[] nameSplit = name.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

        string initials = "";

        foreach (string item in nameSplit)
        {
            initials += item.Substring(0, 1).ToUpper();
        }

        return initials;
    }

    public async Task OnMessagePosted(MessageData message)
    {
        var message1 = message.Map();

        AddOrUpdateMessage(message1);

        loadedMessages.Sort();

        await InvokeAsync(StateHasChanged);

        await NotifyParticipants(message1);
    }

    public async Task OnMessagePostedConfirmed(string messageId)
    {
        var messageVm = loadedMessages.First(x => x.Id == messageId);
        messageVm.Confirmed = true;

        await InvokeAsync(StateHasChanged);
    }

    public async Task OnMessageEdited(string channelId, MessageEditedData data)
    {
        var messageVm = messagesCache.FirstOrDefault(x => x.Id == data.Id);

        if (messageVm is not null)
        {
            messageVm.Content = data.Content;
            messageVm.LastEdited = data.LastEdited;
            messageVm.LastEditedById = data.LastEditedBy.Id;
            messageVm.LastEditedByName = data.LastEditedBy.Name;

            await InvokeAsync(StateHasChanged);
        }
    }

    public Task OnMessageDeleted(string channelId, MessageDeletedData data)
    {
        var messageVm = messagesCache.FirstOrDefault(x => x.Id == data.Id);

        if (messageVm is not null)
        {
            if (data.HardDelete)
            {
                loadedMessages.Remove(messageVm);
                messagesCache.Remove(messageVm);
            }
            else
            {
                messageVm.Content = string.Empty;
                messageVm.Deleted = data.Deleted;
                messageVm.DeletedById = data.DeletedBy.Id;
                messageVm.DeletedByName = data.DeletedBy.Name;
            }

            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    public async Task OnMessageReaction(string channelId, string messageId, MessageReactionData reaction)
    {
        var messageVm = loadedMessages.FirstOrDefault(x => x.Id == messageId);

        if (messageVm is null) return;

        messageVm.Reactions.Add(new Reaction()
        {
            Content = reaction.Content,
            Date = reaction.Date,
            AddedBy = new Participant
            {
                Id = reaction.AddedBy.Id,
                Name = reaction.AddedBy.Name,
                UserId = reaction.AddedBy.UserId
            }
        });

        StateHasChanged();
    }

    public async Task OnMessageReactionRemoved(string channelId, string messageId, string reaction, string participantId)
    {
        var messageVm = loadedMessages.FirstOrDefault(x => x.Id == messageId);

        if (messageVm is null) return;

        // TODO: Pass the person removing the reaction
        var reaction2 = messageVm.Reactions.FirstOrDefault(x => x.Content == reaction && x.AddedBy.Id == participantId);

        if (reaction2 is null) return;

        messageVm.Reactions.Remove(reaction2);

        StateHasChanged();
    }
}