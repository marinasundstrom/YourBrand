using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Groups.NewGroup;

public class AddEditMemberViewModel
{
    [Required]
    public string Name { get; set; }

    public AttendeeRole Role { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public Portal.User User { get; set; }

    public bool? HasVotingRights { get; set; }

    public AddEditMemberViewModel Clone()
    {
        return new AddEditMemberViewModel
        {
            Name = Name,
            Role = Role,
            Email = Email,
            User = User,
            HasVotingRights = HasVotingRights
        };
    }
}