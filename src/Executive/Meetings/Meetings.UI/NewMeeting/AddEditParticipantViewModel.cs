using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.NewMeeting;

public class AddEditParticipantViewModel
{
    [Required]
    public string Name { get; set; }

    public ParticipantRole Role { get; set; } = ParticipantRole.Participant;

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public Portal.User User { get; set; }

    public bool HasVotingRights { get; set; }

    public AddEditParticipantViewModel Clone() 
    {
        return new AddEditParticipantViewModel 
        { 
            Name = Name,
            Role = Role,
            Email = Email, 
            User = User, 
            HasVotingRights = HasVotingRights 
        };
    }
}