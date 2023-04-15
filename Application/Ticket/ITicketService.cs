using Domain._Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application._Ticket
{
    public interface ITicketService
    {
        Task BookTicket(int eventId, string userId, CancellationToken cancellationToken);
        Task BuyTicket(int id, string userId, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<List<Ticket>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<Ticket>> GetUserTicketsAsync(CancellationToken cancellationToken, string userId);
        Task RemoveExpiredBookedTickets(CancellationToken cancellationToken);
        Task UpdateNumberOfTickets(Ticket entity, CancellationToken cancellationToken);
    }
}
