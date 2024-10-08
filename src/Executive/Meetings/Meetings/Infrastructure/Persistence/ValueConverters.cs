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

internal sealed class MeetingParticipantIdConverter : ValueConverter<MeetingParticipantId, string>
{
    public MeetingParticipantIdConverter()
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

internal sealed class VoteIdConverter : ValueConverter<VoteId, int>
{
    public VoteIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}
