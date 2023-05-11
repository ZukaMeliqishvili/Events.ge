using Application._Event;
using Events.ge.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;

namespace Events.ge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(ILogger<HomeController> logger, IEventService eventService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _eventService = eventService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken,int? page = 1)
        {
            var events = await _eventService.GetAllConfirmedEventsAsync(cancellationToken);
            var model = events.Adapt<List<EventVM>>();
            int pageNumber = page ?? 1;
            return View(model.ToPagedList(pageNumber,8));
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