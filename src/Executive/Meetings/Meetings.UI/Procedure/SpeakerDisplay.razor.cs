using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

using MudBlazor;

using YourBrand.Meetings;

namespace YourBrand.Meetings.Procedure;

public partial class SpeakerDisplay : IDiscussionsHubClient
{
    private HubConnection hubConnection = null!;
    private IDiscussionsHub hubProxy = default!;

    private Queue<SpeakerRequest> speakerQueue = new();
    private SpeakerClock? currentSpeakerClock;
    private Timer? countdownTimer;
    private readonly object timerLock = new();

    private int? speakingTimeLimitSeconds;

    [Parameter]
    public int MeetingId { get; set; }

    public SpeakerRequest? CurrentSpeaker { get; set; }

    protected override async Task OnInitializedAsync()
    {
        organization = await OrganizationProvider.GetCurrentOrganizationAsync()!;

        OrganizationProvider.CurrentOrganizationChanged += OnCurrentOrganizationChanged;

        if (hubConnection is not null && hubConnection.State != HubConnectionState.Disconnected)
        {
            await hubConnection.DisposeAsync();
        }

        await RefreshDiscussionAsync();

        hubConnection = new HubConnectionBuilder()
            .WithUrl($"{NavigationManager.BaseUri}api/meetings/hubs/meetings/procedure/?organizationId={organization.Id}&meetingId={MeetingId}", options =>
            {
                options.AccessTokenProvider = async () => await AccessTokenProvider.GetAccessTokenAsync();
            })
            .WithAutomaticReconnect()
            .Build();

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

    private YourBrand.Portal.Services.Organization organization = default!;

    private async void OnCurrentOrganizationChanged(object? sender, EventArgs ev)
    {
        organization = await OrganizationProvider.GetCurrentOrganizationAsync()!;
    }

    public async Task OnSpeakerRequestRevoked(string agendaItemId, string id)
    {
        await RefreshDiscussionAsync();
    }

    public async Task OnSpeakerRequestAdded(string agendaItemId, string id, string attendeeId, string name)
    {
        await RefreshDiscussionAsync();
    }

    public void Dispose()
    {
        OrganizationProvider.CurrentOrganizationChanged -= OnCurrentOrganizationChanged;
        StopTimer();
        if (hubConnection is not null)
        {
            _ = hubConnection.DisposeAsync();
        }
    }

    public Task OnDiscussionStatusChanged(int status)
    {
        return Task.CompletedTask;
    }

    public async Task OnMovedToNextSpeaker(string agendaItemId, string? id)
    {
        await RefreshDiscussionAsync();
    }

    public async Task OnSpeakerTimeExtended(string agendaItemId, string speakerRequestId, int? allocatedSeconds)
    {
        var handled = false;

        if (CurrentSpeaker?.Id == speakerRequestId)
        {
            CurrentSpeaker.AllocatedSpeakingTimeSeconds = allocatedSeconds;
            handled = true;
        }

        foreach (var request in speakerQueue)
        {
            if (request.Id == speakerRequestId)
            {
                request.AllocatedSpeakingTimeSeconds = allocatedSeconds;
                handled = true;
                break;
            }
        }

        if (!handled)
        {
            await RefreshDiscussionAsync();
            return;
        }

        await InvokeAsync(StateHasChanged);
    }

    public async Task OnDiscussionSpeakingTimeChanged(string agendaItemId, int? speakingTimeLimitSeconds)
    {
        await RefreshDiscussionAsync();
    }

    public async Task OnSpeakerClockStarted(string agendaItemId, string speakerRequestId, int elapsedSeconds, DateTimeOffset startedAtUtc)
    {
        if (CurrentSpeaker?.Id != speakerRequestId)
        {
            await RefreshDiscussionAsync();
            return;
        }

        currentSpeakerClock ??= new SpeakerClock();
        currentSpeakerClock.IsRunning = true;
        currentSpeakerClock.ElapsedSeconds = elapsedSeconds;
        currentSpeakerClock.StartedAtUtc = startedAtUtc;

        StartTimer();

        await InvokeAsync(StateHasChanged);
    }

    public async Task OnSpeakerClockStopped(string agendaItemId, string speakerRequestId, int elapsedSeconds)
    {
        if (CurrentSpeaker?.Id != speakerRequestId)
        {
            await RefreshDiscussionAsync();
            return;
        }

        currentSpeakerClock ??= new SpeakerClock();
        currentSpeakerClock.IsRunning = false;
        currentSpeakerClock.ElapsedSeconds = elapsedSeconds;
        currentSpeakerClock.StartedAtUtc = null;

        StopTimer();

        await InvokeAsync(StateHasChanged);
    }

    public async Task OnSpeakerClockReset(string agendaItemId, string speakerRequestId, int elapsedSeconds)
    {
        if (CurrentSpeaker?.Id != speakerRequestId)
        {
            await RefreshDiscussionAsync();
            return;
        }

        currentSpeakerClock ??= new SpeakerClock();
        currentSpeakerClock.IsRunning = false;
        currentSpeakerClock.ElapsedSeconds = elapsedSeconds;
        currentSpeakerClock.StartedAtUtc = null;

        StopTimer();

        await InvokeAsync(StateHasChanged);
    }

    private async Task RefreshDiscussionAsync()
    {
        if (organization is null)
        {
            return;
        }

        try
        {
            var discussion = await DiscussionsClient.GetDiscussionAsync(organization.Id, MeetingId);

            speakerQueue = new Queue<SpeakerRequest>(discussion.SpeakerQueue ?? Array.Empty<SpeakerRequest>());
            CurrentSpeaker = discussion.CurrentSpeaker;
            currentSpeakerClock = discussion.CurrentSpeakerClock;
            speakingTimeLimitSeconds = discussion.SpeakingTimeLimitSeconds;

            RestartTimerBasedOnClock();

            await InvokeAsync(StateHasChanged);
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private void RestartTimerBasedOnClock()
    {
        StopTimer();

        if (CurrentSpeaker is null || currentSpeakerClock is null)
        {
            return;
        }

        if (currentSpeakerClock.IsRunning && currentSpeakerClock.StartedAtUtc.HasValue)
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        lock (timerLock)
        {
            countdownTimer?.Dispose();
            countdownTimer = new Timer(OnTimerTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
    }

    private void StopTimer()
    {
        lock (timerLock)
        {
            countdownTimer?.Dispose();
            countdownTimer = null;
        }
    }

    private void OnTimerTick(object? state)
    {
        _ = InvokeAsync(StateHasChanged);
    }

    private string GetRemainingTimeDisplay()
    {
        if (CurrentSpeaker is null)
        {
            return "--:--";
        }

        var remaining = GetCurrentRemainingTime();
        return remaining is null ? "--:--" : FormatTimeSpan(remaining.Value);
    }

    private string GetClockStatusDisplay()
    {
        if (CurrentSpeaker is null)
        {
            return string.Empty;
        }

        if (currentSpeakerClock is null)
        {
            return "Clock not started";
        }

        var status = currentSpeakerClock.IsRunning ? "Running" : "Paused";
        var elapsed = GetCurrentElapsedTime();
        var elapsedText = elapsed is null ? "--:--" : FormatTimeSpan(elapsed.Value);

        return $"{status} · Elapsed {elapsedText}";
    }

    private TimeSpan? GetCurrentElapsedTime()
    {
        if (CurrentSpeaker is null || currentSpeakerClock is null)
        {
            return null;
        }

        var elapsed = TimeSpan.FromSeconds(currentSpeakerClock.ElapsedSeconds);

        if (currentSpeakerClock.IsRunning && currentSpeakerClock.StartedAtUtc.HasValue)
        {
            var additional = DateTimeOffset.UtcNow - currentSpeakerClock.StartedAtUtc.Value;
            if (additional > TimeSpan.Zero)
            {
                elapsed += additional;
            }
        }

        return elapsed < TimeSpan.Zero ? TimeSpan.Zero : elapsed;
    }

    private TimeSpan? GetCurrentRemainingTime()
    {
        var allocatedSeconds = CurrentSpeaker?.AllocatedSpeakingTimeSeconds;

        if (allocatedSeconds is null)
        {
            return null;
        }

        var allocated = TimeSpan.FromSeconds(allocatedSeconds.Value);
        var elapsed = GetCurrentElapsedTime();

        if (elapsed is null)
        {
            return allocated;
        }

        var remaining = allocated - elapsed.Value;
        return remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
    }

    private string FormatTimeSpan(TimeSpan value)
    {
        return FormatDuration((int)Math.Max(0, Math.Round(value.TotalSeconds)));
    }

    private string FormatDuration(int? seconds)
    {
        if (seconds is null)
        {
            return "Not set";
        }

        var duration = TimeSpan.FromSeconds(seconds.Value);

        if (duration.TotalHours >= 1)
        {
            return $"{(int)duration.TotalHours}h {duration.Minutes:D2}m {duration.Seconds:D2}s";
        }

        if (duration.TotalMinutes >= 1)
        {
            return $"{(int)duration.TotalMinutes}m {duration.Seconds:D2}s";
        }

        return $"{duration.Seconds}s";
    }
}
