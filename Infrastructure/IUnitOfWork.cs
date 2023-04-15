using Infrastructure._ArchivedEvent;
using Infrastructure._Event;
using Infrastructure._Ticket;
using Infrastructure.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IUnitOfWork
    {
        IEventRepository Event { get; }

        IUserRepository User { get; }

        IArchivedEventRepository ArchivedEvent { get; }

        ITicketRepository Ticket { get; }

        Task SaveAsync();
    }
}
