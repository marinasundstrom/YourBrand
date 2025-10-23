using System;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

using MudBlazor;

using YourBrand.Meetings;
using YourBrand.Meetings.Procedure.Elections;

namespace YourBrand.Meetings.Procedure;


public partial class AttendeePage : IMeetingsProcedureHubClient
{
    Meeting? meeting;
    Agenda? agenda;
    AgendaItem? currentAgendaItem;
    Motion? currentMotion;

    private Discussion? discussion;
    private Voting? voting;
    private Election? election;

    HubConnection procedureHub = null!;
    IDiscussionsHub hubProxy = default!;

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
        
        await InitHubs();
    }

    private async Task InitHubs()
    {
        procedureHub = new HubConnectionBuilder().WithUrl($"{NavigationManager.BaseUri}api/meetings/hubs/meetings/procedure/?organizationId={organization.Id}&meetingId={MeetingId}",
        options =>
        {
            options.AccessTokenProvider = async () =>
            {
                return await AccessTokenProvider.GetAccessTokenAsync();
            };
        }).WithAutomaticReconnect().Build();

        hubProxy = procedureHub.ServerProxy<IDiscussionsHub>();
        _ = procedureHub.ClientRegistration<IMeetingsProcedureHubClient>(this);

        procedureHub.Closed += (error) =>
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

        procedureHub.Reconnected += (error) =>
        {
            Snackbar.Add(T["Reconnected"], configure: options =>
            {
                options.Icon = Icons.Material.Filled.Cable;
            });

            return Task.CompletedTask;
        };

        procedureHub.Reconnecting += (error) =>
        {
            Snackbar.Add(T["Reconnecting"], configure: options =>
            {
                options.Icon = Icons.Material.Filled.Cable;
            });

            return Task.CompletedTask;
        };

        await procedureHub.StartAsync();
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
        currentAgendaItem = await MeetingsClient.GetCurrentAgendaItemAsync(organization.Id, MeetingId);

        currentMotion = null;

        if (currentAgendaItem.MotionId is not null)
        {
            currentMotion = await MotionsClient.GetMotionByIdAsync(organization.Id, currentAgendaItem.MotionId.GetValueOrDefault());
        }

        if (currentAgendaItem.State == AgendaItemState.UnderDiscussion)
        {
            discussion = await DiscussionsClient.GetDiscussionAsync(organization.Id, MeetingId);
        }
        else if (currentAgendaItem.State == AgendaItemState.Voting)
        {
            if (currentAgendaItem.Type.Id == (int)AgendaItemTypeEnum.Voting)
            {
                voting = await VotingClient.GetVotingAsync(organization.Id, MeetingId);
            }
            else if (currentAgendaItem.Type.Id == (int)AgendaItemTypeEnum.Election)
            {
                election = await ElectionsClient.GetElectionAsync(organization.Id, MeetingId);
            }
        }
    }

    public void Dispose()
    {
        OrganizationProvider.CurrentOrganizationChanged -= OnCurrentOrganizationChanged;
    }

    public async Task OnMeetingStateChanged(MeetingState state, string? adjournmentMessage)
    {
        meeting = await MeetingsClient.GetMeetingByIdAsync(organization.Id, MeetingId);

        if (meeting.State == MeetingState.Scheduled || meeting.State == MeetingState.Canceled || meeting.State == MeetingState.Completed)
        {
            currentAgendaItem = null;
        }

        StateHasChanged();
    }

    public async Task OnAgendaItemStateChanged(string agendaItemId, AgendaItemState state)
    {
        await LoadAgendaItem();

        StateHasChanged();
    }

    public Task OnVotingStatusChanged(VotingState state)
    {
        return Task.CompletedTask;
    }

    public Task OnElectionStatusChanged(ElectionState state)
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

    public async Task Nominate() 
    {
        DialogParameters parameters = new()
        {
            { nameof(NominateCandidateDialog.OrganizationId), organization.Id },
            { nameof(NominateCandidateDialog.MeetingId), MeetingId }
        };

        var modalRef = await DialogService.ShowAsync<NominateCandidateDialog>("Nominate candidate", parameters);

        var result = await modalRef.Result;

        if (result.Canceled) return;

        var memberModel = (NominateCandidateDialog.ViewModel)result.Data!;

        await AttendeeClient.NominateCandidateAsync(organization.Id, MeetingId,
            new ProposeCandidate {
                AttendeeId = memberModel.Attendee.Id,
                Statement = memberModel.Statement,
            });
    }

    public async Task Register()
    {

    }

    private static string FormatTime(TimeSpan? timeSpan) => timeSpan is null ? "-" : timeSpan.Value.ToString(@"hh\:mm");

    private static string FormatDuration(TimeSpan? duration)
    {
        if (duration is null)
        {
            return "-";
        }

        var minutes = duration.Value.TotalMinutes;

        return minutes % 1 == 0
            ? $"{minutes:0} min"
            : $"{minutes:0.##} min";
    }
}