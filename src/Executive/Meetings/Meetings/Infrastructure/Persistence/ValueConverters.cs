using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Infrastructure.Persistence.ValueConverters;

internal sealed class AgendaIdConverter : ValueConverter<AgendaId, int>
{
    public AgendaIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class AgendaItemIdConverter : ValueConverter<AgendaItemId, string>
{
    public AgendaItemIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MeetingIdConverter : ValueConverter<MeetingId, int>
{
    public MeetingIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MeetingAttendeeIdConverter : ValueConverter<MeetingAttendeeId, string>
{
    public MeetingAttendeeIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class DebateIdConverter : ValueConverter<DebateId, int>
{
    public DebateIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class DebateEntryIdConverter : ValueConverter<DebateEntryId, int>
{
    public DebateEntryIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}


internal sealed class MotionIdConverter : ValueConverter<MotionId, int>
{
    public MotionIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class OperativeClauseIdConverter : ValueConverter<MotionOperativeClauseId, string>
{
    public OperativeClauseIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class VotingSessionIdConverter : ValueConverter<VotingSessionId, string>
{
    public VotingSessionIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class ElectionCandidateIdConverter : ValueConverter<ElectionCandidateId, string>
{
    public ElectionCandidateIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class VoteIdConverter : ValueConverter<VoteId, string>
{
    public VoteIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class SpeakerSessionIdConverter : ValueConverter<SpeakerSessionId, string>
{
    public SpeakerSessionIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class SpeakerRequestIdConverter : ValueConverter<SpeakerRequestId, string>
{
    public SpeakerRequestIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MinutesIdConverter : ValueConverter<MinutesId, int>
{
    public MinutesIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MinutesAttendeeIdConverter : ValueConverter<MinutesAttendeeId, string>
{
    public MinutesAttendeeIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MinutesItemIdConverter : ValueConverter<MinutesItemId, string>
{
    public MinutesItemIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MeetingGroupIdConverter : ValueConverter<MeetingGroupId, int>
{
    public MeetingGroupIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MeetingGroupMemberIdConverter : ValueConverter<MeetingGroupMemberId, string>
{
    public MeetingGroupMemberIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}
