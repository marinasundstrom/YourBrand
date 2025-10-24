using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Features.Agendas;

public interface IAgendaValidator
{
    IReadOnlyDictionary<string, IReadOnlyCollection<AgendaItemValidation>> Validate(Meeting meeting);
}

public sealed record AgendaItemValidation(string Code, string Message, bool IsBlocking);

public sealed class AgendaValidator : IAgendaValidator
{
    public IReadOnlyDictionary<string, IReadOnlyCollection<AgendaItemValidation>> Validate(Meeting meeting)
    {
        if (meeting.Agenda is null)
        {
            return new Dictionary<string, IReadOnlyCollection<AgendaItemValidation>>(StringComparer.Ordinal);
        }

        var validations = new Dictionary<string, IReadOnlyCollection<AgendaItemValidation>>(StringComparer.Ordinal);

        var availableFunctions = new HashSet<int>(
            meeting.Attendees
                .SelectMany(a => a.Functions)
                .Where(f => f.RevokedAt is null)
                .Select(f => f.MeetingFunctionId));

        ValidateItems(meeting.Agenda.Items.OrderBy(x => x.Order), availableFunctions, validations);

        return validations;
    }

    private static void ValidateItems(
        IEnumerable<AgendaItem> items,
        HashSet<int> availableFunctions,
        Dictionary<string, IReadOnlyCollection<AgendaItemValidation>> result)
    {
        foreach (var item in items.OrderBy(x => x.Order))
        {
            var itemValidations = new List<AgendaItemValidation>();

            if (item.Type.RequiresDiscussion && item.DiscussionActions == DiscussionActions.None)
            {
                itemValidations.Add(new AgendaItemValidation(
                    "discussionPhaseRequired",
                    $"{item.Type.Name} requires a discussion phase before it can continue.",
                    true));
            }

            if (item.Type.RequiresVoting && item.VoteActions == VoteActions.None)
            {
                itemValidations.Add(new AgendaItemValidation(
                    "votingPhaseRequired",
                    $"{item.Type.Name} requires a voting phase before it can continue.",
                    true));
            }

            if (item.Type.HandledByFunction is { } function && !availableFunctions.Contains(function.Id))
            {
                var message = function == MeetingFunction.Chairperson
                    ? "Agenda item requires there to be a chairperson."
                    : $"Agenda item requires there to be a {function.Name.ToLowerInvariant()}.";

                itemValidations.Add(new AgendaItemValidation(
                    $"requiredFunction:{function.Id}",
                    message,
                    true));
            }

            if (itemValidations.Count > 0)
            {
                result[item.Id] = itemValidations;
            }

            if (item.Election?.MeetingFunctionId is int meetingFunctionId)
            {
                availableFunctions.Add(meetingFunctionId);
            }

            if (item.SubItems.Any())
            {
                ValidateItems(item.SubItems.OrderBy(x => x.Order), availableFunctions, result);
            }
        }
    }
}
