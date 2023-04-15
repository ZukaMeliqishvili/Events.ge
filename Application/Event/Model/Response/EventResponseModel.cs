﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application._Event.Model.Response
{
    public class EventResponseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int NumberOfTickets { get; set; }

        public double TicketPrice { get; set; }

        public string ImageUrl { get; set; }

        public string UserId { get; set; }

        public bool IsConfirmed { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public int DaysForUpdate { get; set; }
    }
}
