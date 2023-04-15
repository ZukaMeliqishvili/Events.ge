using Domain._Ticket;

namespace Infrastructure._Ticket
{
    public interface ITicketRepository:IBaseRepository<Ticket>
    {
        Task<List<Ticket>> GetAllExpiredAsync(CancellationToken cancellationToken);
        Task<Ticket> GetByUserAndEventIdAsync(string userId, int eventId, CancellationToken cancellationToken);
        Task<List<Ticket>> GetUserTickets(CancellationToken cancellationToken, string userId);
        void RemoveRange(List<Ticket> bookedTickets);
    }
}
