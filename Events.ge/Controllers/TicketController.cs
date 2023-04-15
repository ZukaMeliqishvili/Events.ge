using Application._Event;
using Application._Ticket;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Events.ge.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly string userId;
        private readonly IHttpContextAccessor _contextAccessor;
        public TicketController(IEventService eventService, IHttpContextAccessor httpContextAccessor, ITicketService ticketService)
        {
            _contextAccessor = httpContextAccessor;
            userId = _contextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            _ticketService = ticketService;
        }
        public async Task<IActionResult> Buy(int id, CancellationToken cancellation)
        {
            try
            {
                await _ticketService.BuyTicket(id, userId, cancellation);
                TempData["success"] = "Ticket has been bought successfully";
                return RedirectToAction("Details", "Home", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Details", "Home");
            }
        }
        public async Task<IActionResult> Book(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _ticketService.BookTicket(id, userId, cancellationToken);
                TempData["success"] = "Ticket has been booked successfully";

                return RedirectToAction("Details", "Home", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Details", "Home", new { id = id });
            }
        }
    }
}
