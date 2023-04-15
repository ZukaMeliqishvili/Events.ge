using Application._Event.Model.Request;
using Application._Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Mapster;
using Events.ge.Models;

namespace Events.ge.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string userId;
        public EventController(IEventService eventService, IHttpContextAccessor contextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _eventService = eventService;
            _contextAccessor = contextAccessor;
            userId = userId = _contextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            _hostEnvironment = hostEnvironment;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> GetUnconfirmedEvents(CancellationToken cancellationToken = default)
        {
            var events = await _eventService.GetAllUnconfirmedEventsAsyn(cancellationToken);
            return View("UnconfirmedEvents", events);
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
        {
            try
            {
                //var vm = new EventAdminVM();
                var @event = await _eventService.GetUnconfirmedEventForAdminAsync(id, cancellationToken);
                //vm.Event = @event;
                return View(@event);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("GetUnconfirmedEvents");
            }
        }
        [Authorize(Roles = "Admin,Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Confirm(int id, int bookTimeInMinutes, int daysForUpdate, CancellationToken cancellationToken = default)
        {
            ViewData["bookTimeInMinutes"] = bookTimeInMinutes;
            ViewData["daysForUpdate"] = daysForUpdate;
            try
            {
                await _eventService.ConfirmEvent(id, bookTimeInMinutes, daysForUpdate);
                return RedirectToAction("GetUnconfirmedEvents");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("GetUnconfirmedEvents");
            }
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken = default)
        {
            var model = await _eventService.GetUnconfirmedEventForAdminAsync(id, cancellationToken);
            if (model.ImageUrl != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                var uploads = Path.Combine(wwwRootPath, @"images");
                var oldImagePath = Path.Combine(wwwRootPath, model.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

            }
            await _eventService.Delete(cancellationToken, id);
            return RedirectToAction("GetUnconfirmedEvents");
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(EventRequestModel model, IFormFile file, CancellationToken cancellationToken = default)
        {
            //if(model.StartDate>=model.EndDate)
            //{
            //    ModelState.AddModelError("StartDate", "Event start date cant be higher than event end date");
            //}
            ViewData["file"] = file;
            int titleErrors = ModelState["Title"].Errors.Count;
            int descriptionErrors = ModelState["Description"].Errors.Count;
            int startDateErrors = ModelState["StartDate"].Errors.Count;
            int endDateErrors = ModelState["EndDate"].Errors.Count;
            int numberOfTicketsErrors = ModelState["NumberOfTickets"].Errors.Count;
            int ticketPriceErrors = ModelState["TicketPrice"].Errors.Count;

            if (titleErrors > 0 || descriptionErrors > 0 || startDateErrors > 0 || endDateErrors > 0
                || numberOfTicketsErrors > 0 || ticketPriceErrors > 0)
            {
                TempData["Error"] = "Please enter valid model";
                return RedirectToAction("Create");
            }
            try
            {
                if (file != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images");
                    var extension = Path.GetExtension(file.FileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    model.ImageUrl = @"\images\" + fileName + extension;
                }
                await _eventService.CreateAsync(model, userId, cancellationToken);
                TempData["success"] = "Event successfully created";
                return RedirectToAction("Index", "Home");
            }

            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index", "Home");

            }
        }
        [Authorize]
        public async Task<IActionResult> UserEvents(CancellationToken cancellationToken)
        {
            var events = await _eventService.GetAllUserEventAsync(cancellationToken, userId);
            return View(events);
        }
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken)
        {
            try
            {
                var @event = await _eventService.GetUserEventAsync(id, cancellationToken, userId);
                var vm = @event.Adapt<EventResponseVM>();
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(UserEvents));
            }

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EventResponseVM model, IFormFile file, CancellationToken cancellationToken = default)
        {
            try
            {
                var title = ModelState["Title"].Errors;
                var description = ModelState["Description"].Errors;


                if (title.Count > 0 || description.Count > 0)
                {
                    TempData["Error"] = "Please enter valid model";
                    return RedirectToAction("Update");

                }
                if (file != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images");
                    var extension = Path.GetExtension(file.FileName);
                    if (model.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, model.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    model.ImageUrl = @"\images\" + fileName + extension;
                }
                var requestModel = model.Adapt<EventUpdateRequestModel>();
                await _eventService.UpdateEventAsync(requestModel, model.Id, userId, cancellationToken);
                TempData["success"] = "Event is successfully updated";
                return RedirectToAction(nameof(UserEvents));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(UserEvents));
            }
        }
    }
}
