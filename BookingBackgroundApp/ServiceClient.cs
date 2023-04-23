using Application._Ticket;
using Infrastructure;

namespace BookingBackgroundApp
{
    public class ServiceClient
    {
        private readonly ITicketService _ticketService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServiceClient> _logger;

        public ServiceClient(ITicketService ticketService, IUnitOfWork unitOfWork, ILogger<ServiceClient> logger)
        {
            _ticketService = ticketService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task RemoveExpiredBookings(CancellationToken cancellationToken=default)
        {
            await _ticketService.RemoveExpiredBookedTickets(cancellationToken);
        }
    }
}
