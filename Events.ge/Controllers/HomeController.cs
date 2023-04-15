using Application._Event;
using Events.ge.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Events.ge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        public HomeController(ILogger<HomeController> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var events = await _eventService.GetAllConfirmedEventsAsync(cancellationToken);
            var model = events.Adapt<List<EventVM>>();
            return View(model);
        }
        public async Task<IActionResult> Details(CancellationToken cancellationToken, int id)
        {
            try
            {
                var model = await _eventService.GetConfirmedEventByIdAsync(id, cancellationToken);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}