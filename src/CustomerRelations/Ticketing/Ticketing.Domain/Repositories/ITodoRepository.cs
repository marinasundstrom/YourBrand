using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Ticketing.Domain.Specifications;

namespace YourBrand.Ticketing.Domain.Repositories;

public interface ITicketRepository : IRepository<Ticket, int>
{

}
