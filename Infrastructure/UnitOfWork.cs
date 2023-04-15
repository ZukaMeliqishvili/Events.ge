using Infrastructure._ArchivedEvent;
using Infrastructure._Event;
using Infrastructure._Ticket;
using Infrastructure.User;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork:IUnitOfWork
    {
        private ApplicationDbContext db;

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            Event = new EventRepository(db);
            User = new UserRepository(db);
            ArchivedEvent = new ArchivedEventRepository(db);
            Ticket = new TicketRepositroy(db);
        }

        public IEventRepository Event { get; private set; }

        public IUserRepository User { get; private set; }

        public IArchivedEventRepository ArchivedEvent { get; private set; }

        public ITicketRepository Ticket { get; private set; }


        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
