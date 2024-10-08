using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.MeetingDetails;

public class ParticipantViewModel
{
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    public ParticipantRole Role { get; set; } = ParticipantRole.Participant;

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public Portal.User User { get; set; }

    public bool HasVotingRights { get; set; }

    public bool IsPresent { get; set; }

    public ParticipantViewModel Clone()
    {
        return new ParticipantViewModel
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