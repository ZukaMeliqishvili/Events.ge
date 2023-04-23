using Application._Event;
using Application._Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Events.ge.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetUserTickets(CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetUserTicketsAsync(cancellationToken, userId);
            return View("UserTickets",tickets);
        }
        public async Task<IActionResult> Remove(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _ticketService.DeleteAsync(id, userId, cancellationToken);
                TempData["success"] = "Booking has been removed successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction(nameof(GetUserTickets));


        }
        public async Task<IActionResult> BuyBooked(int id,CancellationToken cancellationToken)
        {
            try
            {
                await _ticketService.BuyBooked(id, userId, cancellationToken);
                TempData["success"] = "Ticket Has been bought successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction(nameof(GetUserTickets));
        }
    }
}
