using Domain._Ticket;
using Domain.User;

namespace Domain.Events
{
    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int NumberOfTickets { get; set; }

        public double TicketPrice { get; set; }

        public string UserId { get; set; }

        public string ImageUrl { get; set; }

        public DateTime ConfirmedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsConfirmed { get; set; }

        public int DaysForUpdate { get; set; }

        public int BookTimeInMinutes { get; set; }

        public ApplicationUser User { get; set; }

        public List<Ticket> Tickets { get; set; }
    }
}