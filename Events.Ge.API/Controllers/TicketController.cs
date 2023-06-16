using Application._Ticket;
using Application._Ticket.Model;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Events.Ge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        protected virtual string GetUserId() => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        [Authorize]
        [HttpPost("book/{id}")]
        public async Task BookTicket(int id, CancellationToken cancellationToken)
        {
            await _ticketService.BookTicket(id, GetUserId(), cancellationToken);
        }
        [Authorize]
        [HttpPut("buy/{id}")]
        public async Task BuyTicket(int id, CancellationToken cancellationToken)
        {
            await _ticketService.BuyTicket(id, GetUserId(), cancellationToken);
        }
        [Authorize]
        [HttpPost("buybooked/{id}")]
        public async Task BuyBooked(int id, CancellationToken cancellationToken)
        {
            await _ticketService.BuyBooked(id, GetUserId(), cancellationToken);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task RemoveBooking(int id, CancellationToken cancellationToken)
        {
            await _ticketService.DeleteAsync(id, GetUserId(), cancellationToken);
        }
        [Authorize]
        [HttpGet]
        public async Task<List<TicketResponseModel>> GetUserTickets(CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetUserTicketsAsync(cancellationToken, GetUserId());
            return tickets.Adapt<List<TicketResponseModel>>();
        }
    }
}
