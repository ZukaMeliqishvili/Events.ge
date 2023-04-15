using Domain.Events;
using Domain.User;

namespace Domain._Ticket
{
    public class Ticket
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
        public bool IsBought { get; set; }

        public DateTime BookedTill { get; set; }
    }
}
