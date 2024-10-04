using YourBrand.Meetings.Features.Users;
using YourBrand.Meetings.Features.Organizations;

namespace YourBrand.Meetings;

public static partial class Mappings
{
    /*
    public static MeetingDto ToDto(this Meeting ticket) => null!;

    public static MeetingParticipantDto ToDto(this MeetingParticipant participant) => new(participant.Id, participant.Name!, null); */

    public static UserDto ToDto(this User user) => new(user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new(user.Id, user.Name);

    public static OrganizationDto ToDto(this Organization user) => new(user.Id, user.Name);

    public static OrganizationDto ToDto2(this Organization user) => new(user.Id, user.Name);
}