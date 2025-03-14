using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public class MeetingGroup : AggregateRoot<MeetingGroupId>, IAuditableEntity<MeetingGroupId>, IHasTenant, IHasOrganization
{
    readonly HashSet<MeetingGroupMember> _members = new HashSet<MeetingGroupMember>();

    protected MeetingGroup()
    {
    }

    public MeetingGroup(MeetingGroupId id, string name, string? description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }

    public Quorum Quorum { get; set; } = new Quorum();

    public IReadOnlyCollection<MeetingGroupMember> Members => _members;

    public MeetingGroupMember? GetMemberById(string id)
    {
        return _members.FirstOrDefault(x => x.Id == id);
    }

    public MeetingGroupMember? GetMemberByUserId(string userId)
    {
        return _members.FirstOrDefault(x => x.UserId == userId);
    }

    public MeetingGroupMember AddMember(string name, string email, AttendeeRole role, UserId? userId, bool? hasSpeakingRights, bool? hasVotingRights)
    {
        int order = 1;

        try
        {
            var last = _members.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var item = new MeetingGroupMember(name, email, role, userId, hasSpeakingRights, hasVotingRights);
        item.Order = order;
        _members.Add(item);
        return item;
    }

    public bool RemoveMember(MeetingGroupMember item)
    {
        int i = 1;
        var r = _members.Remove(item);
        foreach (var member in _members)
        {
            member.Order = i++;
        }
        return r;
    }

    public bool ReorderMember(MeetingGroupMember meetingGroupItem, int newOrderPosition)
    {
        if (!_members.Contains(meetingGroupItem))
        {
            throw new InvalidOperationException("Member does not exist in this group.");
        }

        if (newOrderPosition < 1 || newOrderPosition > _members.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newOrderPosition), "New order position is out of range.");
        }

        int oldOrderPosition = meetingGroupItem.Order;

        if (oldOrderPosition == newOrderPosition)
            return false;

        // Flyttar objektet uppåt i listan
        if (newOrderPosition < oldOrderPosition)
        {
            var itemsToIncrement = Members
                .Where(i => i.Order >= newOrderPosition && i.Order < oldOrderPosition)
                .ToList();

            foreach (var item in itemsToIncrement)
            {
                item.Order += 1;
            }
        }
        // Flyttar objektet nedåt i listan
        else
        {
            var itemsToDecrement = Members
                .Where(i => i.Order > oldOrderPosition && i.Order <= newOrderPosition)
                .ToList();

            foreach (var item in itemsToDecrement)
            {
                item.Order -= 1;
            }
        }

        // Uppdatera order för objektet som flyttas
        meetingGroupItem.Order = newOrderPosition;

        return true;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}