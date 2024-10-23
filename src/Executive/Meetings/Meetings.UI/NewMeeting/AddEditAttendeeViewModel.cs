using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.NewMeeting;

public class AddEditAttendeeViewModel
{
    [Required]
    public string Name { get; set; }

    public AttendeeRole Role { get; set; } = AttendeeRole.Attendee;

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public Portal.User User { get; set; }

    public bool? HasVotingRights { get; set; }

    public AddEditAttendeeViewModel Clone()
    {
        return new AddEditAttendeeViewModel
        {
            Name = Name,
            Role = Role,
            Email = Email,
            User = User,
            HasVotingRights = HasVotingRights
        };
    }
}