using YourBrand.Meetings.Domain.Entities;
using YourBrand.Tenancy;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Repositories;

public interface IMeetingRepository : IRepository<Meeting, MeetingId>
{

}