using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using ChatApp.Theming;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using ChatApp.Features.Chat;

namespace ChatApp.Chat.Channels
{
    public partial class ChannelPage : IChatHubClient
    {
        bool isDarkMode = false;
        string currentUserId = "BS";
        bool isInAdminRole = false;
        Guid? editingMessageId = null;
        MessageViewModel? replyToMessage = null;

        List<MessageViewModel> messagesCache = new List<MessageViewModel>();
        List<MessageViewModel> loadedMessages = new List<MessageViewModel>();

        bool loaded = false;
       
        [Parameter]
        public string? Id { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }  = default!;

        HubConnection hubConnection = null!;
        IChatHub hubProxy = default!;

        UserInfo userInfo = default!;

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            ThemeManager.ColorSchemeChanged += ColorSchemeChanged;
            isDarkMode = ThemeManager.CurrentColorScheme == ColorScheme.Dark;

            StateHasChanged();

            currentUserId = await CurrentUserService.GetUserIdAsync();
            isInAdminRole = await CurrentUserService.IsInRoleAsync("admin");

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

                hubConnection = new HubConnectionBuilder().WithUrl($"https://localhost:5001/hubs/chat?channelId={Id}", options =>
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
                        Snackbar.Add($"{error.Message}", Severity.Error, configure: options => {
                            options.Icon = Icons.Material.Filled.Cable;
                        });
                    }

                    Snackbar.Add(T["Disconnected"], configure: options => {
                        options.Icon = Icons.Material.Filled.Cable;
                    });

                    return Task.CompletedTask;
                };

                hubConnection.Reconnected += (error) =>
                {
                    Snackbar.Add(T["Reconnected"], configure: options => {
                        options.Icon = Icons.Material.Filled.Cable;
                    });

                    return Task.CompletedTask;
                };

                hubConnection.Reconnecting += (error) =>
                {
                    Snackbar.Add(T["Reconnecting"], configure: options => {
                        options.Icon = Icons.Material.Filled.Cable;
                    });

                    return Task.CompletedTask;
                };

                await hubConnection.StartAsync();

                Snackbar.Add(T["Connected"], configure: options => {
                    options.Icon = Icons.Material.Filled.Cable;
                });
            }
            catch (Exception exc)
            {
                Snackbar.Add(exc.Message.ToString(), Severity.Error);
            }
        }

        private async Task LoadMessages()
        {
            var result = await MessagesClient.GetMessagesAsync(Guid.Parse(Id!), 1, 10, null, null);

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
                messageVm.Published = message.Published;
                messageVm.Content = message.Content;
                messageVm.Edited = message.LastEdited;
                messageVm.EditedById = message.LastEditedBy?.Id;
                messageVm.EditedByName = message.LastEditedBy?.Name;
                messageVm.Deleted = message.Deleted;
                messageVm.DeletedById = message.DeletedBy?.Id;
                messageVm.DeletedByName = message.DeletedBy?.Name;
                messageVm.IsFromCurrentUser = message.PublishedBy.Id == currentUserId;

                messageVm.Reactions = message.Reactions.ToList();

                // This is a new incoming message:

                if (message.PublishedBy.Id != currentUserId) 
                {
                    messageVm.Id = message.Id;
                    messageVm.PostedById = message.PublishedBy.Id;
                    messageVm.PostedByName = message.PublishedBy.Name;
                    messageVm.PostedByInitials = GetInitials(message.PublishedBy.Name);
                    messageVm.Content = message.Content;
                    messageVm.ReplyTo = message.ReplyTo is null ? null : GetOrCreateReplyMessageVm(message.ReplyTo);
                    messageVm.Confirmed = true;
                }

                return;
            }

            messageVm = new MessageViewModel
            {
                Id = message.Id,
                PostedById = message.PublishedBy.Id,
                PostedByName = message.PublishedBy.Name,
                PostedByInitials = GetInitials(message.PublishedBy.Name),
                Published = message.Published,
                Edited = message.LastEdited,
                EditedById = message.LastEditedBy?.Id,
                EditedByName = message.LastEditedBy?.Name,
                Content = message.Content,
                IsFromCurrentUser = message.PublishedBy.Id == currentUserId,
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
            var existingMessageVm =  messagesCache.FirstOrDefault(x => x.Id == replyMessage.Id);

            if(existingMessageVm is not null) 
            {
                return existingMessageVm;
            }

            var messageVm = new MessageViewModel
            {
                Id = replyMessage.Id,
                Content = replyMessage.Content,
                Published = replyMessage.Published,
                Deleted = replyMessage.Deleted,
                //IsFromCurrentUser = replyMessage.PublishedBy.Id == currentUserId,
                Confirmed = true
            };

            messagesCache.Add(messageVm);

            return messageVm;
        }

        private async Task NotifyParticipants(Message message)
        {
            if (message.ReplyTo is null)
            {
                if (message.PublishedBy.Id == currentUserId)
                {
                    await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
                }
                else
                {
                    // TODO: Only display when outside viewport

                    Snackbar.Add($"{message.PublishedBy.Name} said: \"{message.Content}\"", Severity.Normal, options =>
                    {
                        options.Onclick = async (sb) =>
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
        public string Text { get; set; } = default !;

        async Task Send()
        {
            if(string.IsNullOrWhiteSpace(Text))  
            {
                return;
            }

            if(editingMessageId is not null) 
            {
                await UpdateMessage();

                return;
            }

            var message = new MessageViewModel() 
            {
                Id = Guid.Empty,
                Published = DateTimeOffset.UtcNow,
                PostedById = currentUserId,
                PostedByName = userInfo.Name,
                PostedByInitials = GetInitials(userInfo.Name), // TODO: Fix with my name,
                ReplyTo = replyToMessage,
                IsFromCurrentUser = true,
                Content = Text
            };

            loadedMessages.Add(message);
            messagesCache.Add(message);

            message.Id = await hubConnection.InvokeAsync<Guid>("PostMessage", Id, replyToMessage?.Id, Text);

            Text = string.Empty;
            replyToMessage = null;

            await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
        }

        private async Task UpdateMessage()
        {
            var messageVm = loadedMessages.FirstOrDefault(x => x.Id == editingMessageId);

            if(messageVm is not null) 
            {
                messageVm.Content = Text;

                await MessagesClient.EditMessageAsync(editingMessageId.GetValueOrDefault(), Text);

                Text = string.Empty;
                editingMessageId = null;
            }
        }

        async Task DeleteMessage(MessageViewModel messageVm) 
        {
            await MessagesClient.DeleteMessageAsync(messageVm.Id);

            if(messageVm is not null) 
            {
                if(replyToMessage?.Id == messageVm.Id)
                {
                    AbortReplyToMessage();
                }

                if(editingMessageId == messageVm.Id)
                {
                    AbortEditMessage();
                }
            }
        }

        void EditMessage(MessageViewModel messageVm) 
        {
            replyToMessage = null;
            editingMessageId = messageVm.Id;
            Text = messageVm.Content;
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

            await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
        }

        void AbortReplyToMessage() 
        {
            replyToMessage = null;
            Text = string.Empty;
        }

        async Task React(MessageViewModel messageVm, string reaction)
        {
            await MessagesClient.ReactAsync(messageVm.Id, reaction);
        }

        private bool IsFirst(MessageViewModel currentMessage)
        {
            var index = loadedMessages.IndexOf(currentMessage);
            if (index == 0)
            {
                return true;
            }

            var previousMessage = loadedMessages[index - 1];

            if(previousMessage.PostedById != currentMessage.PostedById) 
            {
                return true;
            }

            if (!(currentMessage.Published.Year == previousMessage.Published.Year && currentMessage.Published.Month == previousMessage.Published.Month && currentMessage.Published.Day == previousMessage.Published.Day && currentMessage.Published.Hour == previousMessage.Published.Hour && currentMessage.Published.Minute == previousMessage.Published.Minute))
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

            if(nextMessage.PostedById != currentMessage.PostedById) 
            {
                return true;
            }

            if (!(currentMessage.Published.Year == nextMessage.Published.Year && currentMessage.Published.Month == nextMessage.Published.Month && currentMessage.Published.Day == nextMessage.Published.Day && currentMessage.Published.Hour == nextMessage.Published.Hour && currentMessage.Published.Minute == nextMessage.Published.Minute))
            {
                return true;
            }

            return nextMessage.PostedById != currentMessage.PostedById;
        }

        static string GetInitials(string name)
        {                       
            // StringSplitOptions.RemoveEmptyEntries excludes empty spaces returned by the Split method

            string[] nameSplit = name.Split(new string[] { "," , " "}, StringSplitOptions.RemoveEmptyEntries);
                        
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

            StateHasChanged();

            await NotifyParticipants(message1);
        }

        public Task OnMessagePostedConfirmed(Guid messageId)
        {
            var messageVm = loadedMessages.First(x => x.Id == messageId);
            messageVm.Confirmed = true;

            StateHasChanged();

            return Task.CompletedTask;
        }

        public Task OnMessageEdited(Guid channelId, MessageEditedData data)
        {
            var messageVm = messagesCache.FirstOrDefault(x => x.Id == data.Id);

            if(messageVm is not null) 
            {
                messageVm.Content = data.Content;
                messageVm.Edited = data.LastEdited;
                messageVm.EditedById = data.LastEditedBy.Id;
                messageVm.EditedByName = data.LastEditedBy.Name;

                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        public Task OnMessageDeleted(Guid channelId, MessageDeletedData data)
        {
            var messageVm = messagesCache.FirstOrDefault(x => x.Id == data.Id);

            if(messageVm is not null) 
            {
                //messages.Remove(messageVm);

                messageVm.Content = string.Empty;
                messageVm.Deleted = data.Deleted;
                messageVm.DeletedById = data.DeletedBy.Id;
                messageVm.DeletedByName = data.DeletedBy.Name;

                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        public async Task OnMessageReaction(Guid channelId, Guid messageId, MessageReactionData reaction)
        {
            var messageVm = loadedMessages.FirstOrDefault(x => x.Id == messageId);

            if(messageVm is null) return;

            messageVm.Reactions.Add(new Reaction() 
            {
                Content = reaction.Content,
                Date = reaction.Date,
                User = new User 
                {
                    Id = reaction.User.Id,
                    Name = reaction.User.Name
                }
            });

            StateHasChanged();
        }

        public async Task OnMessageReactionRemoved(Guid channelId, Guid messageId, string reaction, string userId)
        {
            var messageVm = loadedMessages.FirstOrDefault(x => x.Id == messageId);
            
            if(messageVm is null) return;

            // TODO: Pass the person removing the reaction
            var reaction2 = messageVm.Reactions.FirstOrDefault(x => x.Content == reaction && x.User.Id == userId);

            if(reaction2 is null) return;

            messageVm.Reactions.Remove(reaction2);

            StateHasChanged();
        }
    }
}