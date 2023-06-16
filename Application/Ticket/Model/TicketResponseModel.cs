using Domain.Events;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application._Ticket.Model
{
    public class TicketResponseModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int EventId { get; set; }

        public bool IsBought { get; set; }

        public DateTime BookedTill { get; set; }
    }
}
