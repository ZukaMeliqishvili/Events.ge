using Domain._Ticket;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure._Ticket
{
    public class TicketRepositroy:BaseRepository<Ticket>,ITicketRepository
    {
        private readonly ApplicationDbContext _db;

        public TicketRepositroy(ApplicationDbContext db)
          : base(db)
        {
            _db = db;
        }

        public async Task<Ticket> GetByUserAndEventIdAsync(string userId, int eventId, CancellationToken cancellationToken)
        {
            var query = _db.Tickets.Where(x => x.UserId == userId && x.EventId == eventId && x.IsBought==false);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Ticket>> GetAllExpiredAsync(CancellationToken cancellationToken)
        {
            IQueryable<Ticket> query = _db.Tickets.Where(x => x.BookedTill < DateTime.Now && x.IsBought==false);
            return await query.ToListAsync(cancellationToken);
        }
        public async Task<List<Ticket>> GetUserTickets(CancellationToken cancellationToken,string userId)
        {
           var tickets = _db.Tickets.Where(x => x.UserId == userId).Include(x=>x.Event);
           return await tickets.ToListAsync();
        }
        public void RemoveRange(List<Ticket> bookedTickets)
        {
            _db.Tickets.RemoveRange(bookedTickets);
        }
    }
}
