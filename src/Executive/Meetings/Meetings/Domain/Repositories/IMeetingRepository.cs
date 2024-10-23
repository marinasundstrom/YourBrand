using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Repositories;

public interface IMeetingRepository : IRepository<Meeting, MeetingId>
{

}