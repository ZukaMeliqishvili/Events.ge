
using Domain.User;

namespace Domain.Events
{
    public class ArchivedEvent
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string UserId { get; set; }

        public string ImageUrl { get; set; }

        public DateTime ConfirmedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ApplicationUser User { get; set; }
    }
}
