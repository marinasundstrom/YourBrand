

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

using MudBlazor;

using YourBrand.Meetings;

namespace YourBrand.Meetings.Procedure;


public partial class DisplayPage : IMeetingsProcedureHubClient
{
    Meeting? meeting;
    Agenda? agenda;
    AgendaItem? agendaItem;
    Motion? currentMotion;

    HubConnection hubConnection = null!;
    IMeetingsProcedureHub hubProxy = default!;

    [Parameter]
    public int MeetingId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        organization = await OrganizationProvider.GetCurrentOrganizationAsync()!;

        OrganizationProvider.CurrentOrganizationChanged += OnCurrentOrganizationChanged;

        var currentUserId = await UserContext.GetUserId()!;

        meeting = await MeetingsClient.GetMeetingByIdAsync(organization.Id, MeetingId);

        await LoadAgenda();

        if (meeting.CurrentAgendaItemIndex is not null)
        {
            await LoadAgendaItem();
        }

        if (hubConnection is not null && hubConnection.State != HubConnectionState.Disconnected)
        {
            await hubConnection.DisposeAsync();
        }

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
        _ = hubConnection.ClientRegistration<IMeetingsProcedureHubClient>(this);

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
        /*}
        catch (Exception exc)
        {
            Snackbar.Add(exc.Message.ToString(), Severity.Error);

            throw;
        }*/
    }

    YourBrand.Portal.Services.Organization organization = default!;

    private async void OnCurrentOrganizationChanged(object? sender, EventArgs ev)
    {
        organization = await OrganizationProvider.GetCurrentOrganizationAsync()!;
    }

    public async Task OnAgendaItemChanged(string agendaItemId)
    {
        await LoadAgendaItem();

        StateHasChanged();
    }

    public async Task OnAgendaUpdated()
    {
        await LoadAgenda();

        StateHasChanged();
    }

    private async Task LoadAgenda()
    {
        agenda = await MeetingsClient.GetMeetingAgendaAsync(organization.Id, MeetingId);
    }

    private async Task LoadAgendaItem()
    {
        agendaItem = await MeetingsClient.GetCurrentAgendaItemAsync(organization.Id, MeetingId);

        currentMotion = null;

        if (agendaItem.MotionId is not null)
        {
            currentMotion = await MotionsClient.GetMotionByIdAsync(organization.Id, agendaItem.MotionId.GetValueOrDefault());
        }
    }

    public void Dispose()
    {
        OrganizationProvider.CurrentOrganizationChanged -= OnCurrentOrganizationChanged;
    }

    public Task OnMeetingStateChanged(MeetingState state)
    {
        if (state == MeetingState.Scheduled || state == MeetingState.Canceled || state == MeetingState.Completed)
        {
            agendaItem = null;
        }

        StateHasChanged();

        return Task.CompletedTask;
    }

    public async Task OnAgendaItemStateChanged(string agendaItemId, AgendaItemState state)
    {
        await LoadAgendaItem();

        if(state == AgendaItemState.UnderDiscussion) 
        {
            Console.WriteLine("Under discussion");
        }
        else if (state == AgendaItemState.Voting)
        {

            Console.WriteLine("Voting");
        }

        StateHasChanged();
    }

    public Task OnVotingStatusChanged(int status)
    {
        return Task.CompletedTask;
    }

    public Task OnElectionStatusChanged(int status)
    {
        return Task.CompletedTask;
    }

    public Task OnDiscussionStatusChanged(int status)
    {
        return Task.CompletedTask;
    }

    public Task OnSpeakerRequestRevoked(string agendaItemId, string id)
    {
        return Task.CompletedTask;
    }

    public Task OnSpeakerRequestAdded(string agendaItemId, string id, string attendeeId)
    {
        return Task.CompletedTask;
    }
}