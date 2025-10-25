using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

using YourBrand;
using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Features.Minutes;

internal static class MinutesRecordingExtensions
{
    public static async Task<Domain.Entities.Minutes> EnsureMinutesAsync(
        this IApplicationDbContext context,
        Meeting meeting,
        CancellationToken cancellationToken)
    {
        if (meeting.Minutes is not null)
        {
            UpdateMetadata(meeting.Minutes, meeting);
            return meeting.Minutes;
        }

        var nextId = 1;

        try
        {
            nextId = await context.Minutes
                .InOrganization(meeting.OrganizationId)
                .MaxAsync(x => x.Id, cancellationToken) + 1;
        }
        catch
        {
            // No minutes created yet for the organization.
        }

        var minutes = new Domain.Entities.Minutes(nextId)
        {
            TenantId = meeting.TenantId,
            OrganizationId = meeting.OrganizationId,
            MeetingId = meeting.Id,
            Title = meeting.Title,
            Location = meeting.Location,
            Description = meeting.Description,
            Date = meeting.StartedAt ?? meeting.ScheduledAt,
            Started = meeting.StartedAt ?? DateTimeOffset.UtcNow
        };

        meeting.Minutes = minutes;

        context.Minutes.Add(minutes);

        return minutes;
    }

    public static async Task<MinutesItem?> RecordAgendaItemMinutesAsync(
        this IApplicationDbContext context,
        Meeting meeting,
        AgendaItem agendaItem,
        CancellationToken cancellationToken)
    {
        if (agendaItem.State != AgendaItemState.Completed)
        {
            return null;
        }

        var minutes = await context.EnsureMinutesAsync(meeting, cancellationToken);

        UpdateMetadata(minutes, meeting);

        var (heading, details, motionId) = BuildMinutesContent(agendaItem);

        var existing = minutes.Items.FirstOrDefault(x => x.AgendaItemId == agendaItem.Id);

        if (existing is null)
        {
            existing = minutes.AddItem(
                agendaItem.Type,
                meeting.Agenda?.Id,
                agendaItem.Id,
                heading,
                details);
        }
        else
        {
            existing.Type = agendaItem.Type;
            existing.Heading = heading;
            existing.Details = details;
        }

        existing.MotionId = motionId;

        return existing;
    }

    private static void UpdateMetadata(Domain.Entities.Minutes minutes, Meeting meeting)
    {
        minutes.Title = meeting.Title;
        minutes.Location = meeting.Location;
        minutes.Description = meeting.Description;

        if (meeting.StartedAt is not null)
        {
            minutes.Started ??= meeting.StartedAt;
            minutes.Date = meeting.StartedAt;
        }
        else if (minutes.Date is null)
        {
            minutes.Date = meeting.ScheduledAt;
        }
    }

    private static (string Heading, string Details, MotionId? MotionId) BuildMinutesContent(AgendaItem agendaItem)
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(agendaItem.Description))
        {
            builder.AppendLine(agendaItem.Description.Trim());
            builder.AppendLine();
        }

        if (agendaItem.Type == AgendaItemType.Election && agendaItem.Election is { } election)
        {
            AppendElectionResults(builder, election);
        }
        else if (agendaItem.Voting is { } voting &&
                 voting.State == VotingState.ResultReady)
        {
            AppendVotingResults(builder, voting);
        }
        else
        {
            builder.AppendLine($"Agenda item \"{agendaItem.Title}\" was completed.");
        }

        builder.AppendLine($"Completed at {DateTimeOffset.UtcNow:u}.");

        var details = builder.ToString().Trim();

        return (agendaItem.Title, details, agendaItem.MotionId);
    }

    private static void AppendVotingResults(StringBuilder builder, Voting voting)
    {
        var forVotes = voting.Votes.Count(v => v.Option == VoteOption.For);
        var againstVotes = voting.Votes.Count(v => v.Option == VoteOption.Against);
        var abstainVotes = voting.Votes.Count(v => v.Option == VoteOption.Abstain);

        builder.AppendLine("Voting results:");
        builder.AppendLine($"- For: {forVotes}");
        builder.AppendLine($"- Against: {againstVotes}");
        builder.AppendLine($"- Abstain: {abstainVotes}");
        builder.AppendLine();
        builder.AppendLine(voting.HasPassed ? "Outcome: Motion carried." : "Outcome: Motion failed.");
    }

    private static void AppendElectionResults(StringBuilder builder, Election election)
    {
        builder.AppendLine("Election results:");

        var activeCandidates = election.Candidates
            .Where(candidate => candidate.WithdrawnAt is null)
            .OrderByDescending(candidate => election.Ballots.Count(ballot => ballot.SelectedCandidateId == candidate.Id))
            .ToList();

        if (activeCandidates.Count == 0)
        {
            builder.AppendLine("- No candidates available.");
        }
        else
        {
            foreach (var candidate in activeCandidates)
            {
                var votes = election.Ballots.Count(ballot => ballot.SelectedCandidateId == candidate.Id);
                builder.AppendLine($"- {candidate.Name}: {votes} vote(s)");
            }
        }

        builder.AppendLine();

        if (election.ElectedCandidate is not null)
        {
            builder.AppendLine($"Elected: {election.ElectedCandidate.Name}.");
        }
        else if (election.State == ElectionState.RedoRequired)
        {
            builder.AppendLine("Result: A tie occurred. A new election is required.");
        }
        else
        {
            builder.AppendLine("Result: No candidate reached the required threshold.");
        }
    }
}
