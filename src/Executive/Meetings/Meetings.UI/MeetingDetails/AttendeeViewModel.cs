using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.MeetingDetails;

public class AttendeeViewModel
{
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    public AttendeeRole Role { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public Portal.User User { get; set; }

    public bool? HasVotingRights { get; set; }

    public bool IsPresent { get; set; }

    public AttendeeViewModel Clone()
    {
        return new AttendeeViewModel
        {
            Id = Id,
            Name = Name,
            Role = Role,
            Email = Email,
            User = User,
            HasVotingRights = HasVotingRights,
            IsPresent = IsPresent
        };
    }
}