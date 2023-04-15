using Domain._Ticket;
using Domain.Events;
using Microsoft.AspNetCore.Identity;

namespace Domain.User
{
    public class ApplicationUser:IdentityUser
    {
        public List<Event> Events { get; set; }

        public List<ArchivedEvent> ArchivedEvents { get; set; }

        public List<Ticket> Tickets { get; set; }
    }
}
