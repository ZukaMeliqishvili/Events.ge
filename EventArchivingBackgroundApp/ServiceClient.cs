using Application._Event;
using Application._Ticket;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventArchivingBackgroundApp
{
    public class ServiceClient
    {
        private readonly IEventService _eventService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServiceClient> _logger;
        private readonly ITicketService _ticketService;

        public ServiceClient(IEventService eventService, IUnitOfWork unitOfWork, ILogger<ServiceClient> logger, ITicketService ticketService)
        {
            _eventService = eventService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _ticketService = ticketService;
        }

        public async Task RemoveExpiredBookings(CancellationToken cancellationToken = default)
        {
            await _eventService.MoveEventsToArchive(cancellationToken);
        }
    }
}
