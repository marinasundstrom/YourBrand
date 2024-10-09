using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum AgendaItemState
{
    Pending,
    UnderDiscussion,
    Voting,
    Completed
}

public class AgendaItem : Entity<AgendaItemId>, IAuditable, IHasTenant, IHasOrganization
{
    public AgendaItem(string title, string description)
    : base(new AgendaItemId())
    {
        Title = title;
        Description = description;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public AgendaId AgendaId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public AgendaItemState State { get; set; } = AgendaItemState.Pending;
    public int Order { get; set; }
    public MotionId? MotionId { get; set; }

    public void StartDiscussion()
    {
        if (State != AgendaItemState.Pending)
        {
            throw new InvalidOperationException("Cannot start discussion.");
        }

        State = AgendaItemState.UnderDiscussion;
    }

    public void StartVoting()
    {
        if (State != AgendaItemState.UnderDiscussion)
        {
            throw new InvalidOperationException("Cannot start voting.");
        }

        State = AgendaItemState.Voting;
    }

    public void CompleteAgendaItem()
    {
        if (State != AgendaItemState.Voting)
        {
            throw new InvalidOperationException("Agenda item voting not completed.");
        }

        State = AgendaItemState.Completed;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}