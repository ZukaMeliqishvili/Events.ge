using Application._Event;
using Application._Event.Model.Request;
using Application._Event.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Events.Ge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        protected string GetUserId() => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        [HttpGet("events")]
        public async Task<List<EventResponseModel>> GetAllConfirmedEvent(CancellationToken cancellationToken)
        {
            return await _eventService.GetAllConfirmedEventsAsync(cancellationToken);
        }
        [HttpGet("events/{id}")]
        public async Task<EventResponseModel> GetConfirmedEvent(int id, CancellationToken cancellationToken)
        {
            return await _eventService.GetConfirmedEventByIdAsync(id, cancellationToken);
        }
        [Authorize]
        [HttpGet("userEvents")]

        public async Task<List<EventResponseModel>> GetAllUserEvents(CancellationToken cancellationToken)
        {
            return await _eventService.GetAllUserEventAsync(cancellationToken, GetUserId());
        }
        [Authorize]
        [HttpGet("userEvents/{id}")]
        public async Task<EventResponseModel> GetUserEvent(int id,CancellationToken cancellationToken)
        {
            return await _eventService.GetUserEventAsync(id, cancellationToken, GetUserId());
        }

        [Authorize]
        [HttpPost]
        public async Task Create(EventRequestModel @event, CancellationToken cancellationToken)
        {
            await _eventService.CreateAsync(@event, GetUserId(), cancellationToken);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task Update(
          EventUpdateRequestModel @event,
          int id, CancellationToken cancellationToken)
        {
            await _eventService.UpdateEventAsync(@event, id, GetUserId(), cancellationToken);
        }
    }
}
