using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Repositories;

public interface ITicketRepository : IRepository<Ticket, TicketId>
{

}