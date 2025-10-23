

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

using System.Linq;

using MudBlazor;

using YourBrand.Meetings;

namespace YourBrand.Meetings.Procedure;


public partial class SpeakerDisplay : IDiscussionsHubClient
{
    HubConnection hubConnection = null!;
    IDiscussionsHub hubProxy = default!;

    readonly SpeakerRequest? currentSpeaker;
    Queue<SpeakerRequest> speakerQueue = new Queue<SpeakerRequest>();

    [Parameter]
    public int MeetingId { get; set; }

    public SpeakerRequest? CurrentSpeaker { get; set; }

    protected override async Task OnInitializedAsync()
    {
        organization = await OrganizationProvider.GetCurrentOrganizationAsync()!;

        OrganizationProvider.CurrentOrganizationChanged += OnCurrentOrganizationChanged;

        var currentUserId = await UserContext.GetUserId()!;

        if (hubConnection is not null && hubConnection.State != HubConnectionState.Disconnected)
        {
            await hubConnection.DisposeAsync();
        }

        var discussion = await DiscussionsClient.GetDiscussionAsync(organization!.Id, MeetingId);

        foreach(var speaker in discussion.SpeakerQueue) 
        {
            speakerQueue.Enqueue(speaker);
        }

        CurrentSpeaker = discussion.CurrentSpeaker;

        hubConnection = new
        HubConnectionBuilder().WithUrl($"{NavigationManager.BaseUri}api/meetings/hubs/meetings/procedure/?organizationId={organization.Id}&meetingId={MeetingId}",
        options =>
        {
            options.AccessTokenProvider = async () =>
            {
                return await AccessTokenProvider.GetAccessTokenAsync();
            };
        }).WithAutomaticReconnect().Build();

        hubProxy = hubConnection.ServerProxy<IMeetingsProcedureHub>();
        _ = hubConnection.ClientRegistration<IDiscussionsHubClient>(this);

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
    }

    YourBrand.Portal.Services.Organization organization = default!;

    private async void OnCurrentOrganizationChanged(object? sender, EventArgs ev)
    {
        organization = await OrganizationProvider.GetCurrentOrganizationAsync()!;
    }

    public Task OnSpeakerRequestRevoked(string agendaItemId, string id)
    {
        var list = speakerQueue.Where(x => x.Id != id).ToList();
        speakerQueue = new Queue<SpeakerRequest>(list);
        if (CurrentSpeaker?.Id == id)
        {
            CurrentSpeaker = null;
        }

        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task OnSpeakerRequestAdded(string agendaItemId, string id, string attendeeId, string name)
    {
        speakerQueue.Enqueue(new SpeakerRequest() { Id = id, Name = name, AttendeeId = attendeeId, });

        Console.WriteLine("Added");

        StateHasChanged();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        OrganizationProvider.CurrentOrganizationChanged -= OnCurrentOrganizationChanged;
    }

    public Task OnDiscussionStatusChanged(int status)
    {
        return Task.CompletedTask;
    }

    public Task OnMovedToNextSpeaker(string agendaItemId, string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            CurrentSpeaker = null;
            speakerQueue.Clear();

            StateHasChanged();

            return Task.CompletedTask;
        }

        var originalQueue = speakerQueue.ToList();
        SpeakerRequest? nextSpeaker = null;
        var remaining = new Queue<SpeakerRequest>();
        var found = false;

        foreach (var candidate in originalQueue)
        {
            if (!found && candidate.Id == id)
            {
                nextSpeaker = candidate;
                found = true;
                continue;
            }

            if (found)
            {
                remaining.Enqueue(candidate);
            }
        }

        if (!found)
        {
            speakerQueue = new Queue<SpeakerRequest>(originalQueue);

            StateHasChanged();

            return Task.CompletedTask;
        }

        speakerQueue = remaining;

        CurrentSpeaker = nextSpeaker;

        StateHasChanged();

        return Task.CompletedTask;
    }
}