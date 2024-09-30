using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Infrastructure.Persistence.ValueConverters;

internal sealed class TicketIdConverter : ValueConverter<TicketId, int>
{
    public TicketIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class TicketParticipantIdConverter : ValueConverter<TicketParticipantId, string>
{
    public TicketParticipantIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class ProjectIdConverter : ValueConverter<ProjectId, int>
{
    public ProjectIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}