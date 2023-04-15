
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Application._Event.Model.Request
{
    public class EventRequestModel
    {
        [Required, NotNull]
        [MinLength(5)]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required, NotNull]
        [MinLength(10)]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required, NotNull]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [Required, NotNull]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of tickets must be equal or more than 1")]
        [DisplayName("Number of tickets")]
        public int NumberOfTickets { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be equal or more than 1")]
        [DisplayName("Price")]
        public double TicketPrice { get; set; }

        public string ImageUrl { get; set; }
    }
}
