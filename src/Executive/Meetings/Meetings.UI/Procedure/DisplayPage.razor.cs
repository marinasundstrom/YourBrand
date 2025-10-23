using System;

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
        if (agenda is null)
        {
            agenda = await MeetingsClient.GetMeetingAgendaAsync(organization.Id, MeetingId);
        }
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
        else if (state == AgendaItemState.Canceled)
        {
            Console.WriteLine("Canceled");
        }
        else if (state == AgendaItemState.Skipped)
        {
            Console.WriteLine("Skipped");
        }
        else if (state == AgendaItemState.Postponed)
        {
            Console.WriteLine("Postponed");
        }
        else if (state == AgendaItemState.Completed)
        {
            Console.WriteLine("Completed");
        }

        StateHasChanged();
    }

    public Task OnVotingStatusChanged(VotingState state)
    {
        if (state == VotingState.Voting)
        {
            Console.WriteLine("Voting");
        }
        else if (state == VotingState.RedoRequired)
        {
            Console.WriteLine("Voting redo required");
        }
        else if (state == VotingState.ResultReady)
        {
            Console.WriteLine("Voting result ready");
        }
        else if (state == VotingState.Completed)
        {
            Console.WriteLine("Voting completed");
        }

        return Task.CompletedTask;
    }

    public Task OnElectionStatusChanged(ElectionState state)
    {
        if (state == ElectionState.Voting)
        {
            Console.WriteLine("Election");
        }
        else if (state == ElectionState.RedoRequired)
        {
            Console.WriteLine("Election redo required");
        }
        else if (state == ElectionState.ResultReady)
        {
            Console.WriteLine("Election result ready");
        }
        else if (state == ElectionState.Completed)
        {
            Console.WriteLine("Election completed");
        }

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

    public Task OnSpeakerRequestAdded(string agendaItemId, string id, string attendeeId, string name)
    {
        return Task.CompletedTask;
    }

    public Task OnMovedToNextSpeaker(string agendaItemId, string? id)
    {
        return Task.CompletedTask;
    }

    public Task OnSpeakerTimeExtended(string agendaItemId, string speakerRequestId, int? allocatedSeconds)
    {
        return Task.CompletedTask;
    }

    public Task OnDiscussionSpeakingTimeChanged(string agendaItemId, int? speakingTimeLimitSeconds)
    {
        return Task.CompletedTask;
    }

    public Task OnSpeakerClockStarted(string agendaItemId, string speakerRequestId, int elapsedSeconds, DateTimeOffset startedAtUtc)
    {
        return Task.CompletedTask;
    }

    public Task OnSpeakerClockStopped(string agendaItemId, string speakerRequestId, int elapsedSeconds)
    {
        return Task.CompletedTask;
    }

    public Task OnSpeakerClockReset(string agendaItemId, string speakerRequestId, int elapsedSeconds)
    {
        return Task.CompletedTask;
    }
}