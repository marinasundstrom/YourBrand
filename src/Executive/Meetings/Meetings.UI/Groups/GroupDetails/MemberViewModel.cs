using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Groups.GroupDetails;

public class MemberViewModel
{
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    public AttendeeRole Role { get; set; } = AttendeeRole.Participant;

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public Portal.User User { get; set; }

    public bool HasVotingRights { get; set; }

    public bool IsPresent { get; set; }

    public MemberViewModel Clone()
    {
        return new MemberViewModel
        {
            Name = Name,
            Role = Role,
            Email = Email,
            User = User,
            HasVotingRights = HasVotingRights,
            IsPresent = IsPresent
        };
    }
}