using Application._Ticket;
using Application._Event.Model.Request;
using Application._Event.Model.Response;
using Domain.Events;
using Infrastructure;
using Mapster;
namespace Application._Event
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketService _ticketService;

        public EventService(IUnitOfWork unitOfWork, ITicketService ticketService)
        {
            _unitOfWork = unitOfWork;
            _ticketService = ticketService;
        }

        public async Task CreateAsync(EventRequestModel @event, string userId, CancellationToken cancellationToken)
        {
            if (@event == null)
                throw new ArgumentNullException("Model can not be null");
            if (@event.StartDate > @event.EndDate)
                throw new Exception("Event startd date must be higher than end date");
            Event entity = @event.Adapt<Event>();
            entity.UserId = userId;
            await _unitOfWork.Event.CreateAsync(cancellationToken, entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<EventResponseModel>> GetAllConfirmedEventsAsync(
          CancellationToken cancellationToken)
        {
            List<Event> events = await _unitOfWork.Event.GetAllActiveEvents(cancellationToken);
            List<EventResponseModel> confirmedEventsAsync = events.Adapt<List<EventResponseModel>>();
            return confirmedEventsAsync;
        }

        public async Task<List<EventResponseModel>> GetAllUnconfirmedEventsAsyn(
          CancellationToken cancellationToken)
        {
            List<Event> events = await this._unitOfWork.Event.GetAllUnconfirmedEvents(cancellationToken);
            List<EventResponseModel> unconfirmedEventsAsyn = events.Adapt<List<EventResponseModel>>();
            return unconfirmedEventsAsyn;
        }

        public async Task<List<EventResponseModel>> GetAllUserEventAsync(CancellationToken cancellationToken, string userId)
        {
            List<Event> events = await this._unitOfWork.Event.GetAllEventsByUser(cancellationToken, userId);
            List<EventResponseModel> allUserEventAsync = events.Adapt<List<EventResponseModel>>();

            return allUserEventAsync;
        }
        public async Task Delete(CancellationToken cancellation, int id)
        {
            var obj = await _unitOfWork.Event.GetByIdAsync(cancellation, id);
            if (obj is null)
            {
                throw new Exception("Event not found");
            }
            if (obj.IsConfirmed)
            {
                throw new Exception("Event is already confirmed");
            }
            await _unitOfWork.Event.DeleteAsync(cancellation, id);
            await _unitOfWork.SaveAsync();
        }
        public async Task<EventResponseModel> GetConfirmedEventByIdAsync(
          int id,
          CancellationToken cancellationToken)
        {
            Event @event = await _unitOfWork.Event.GetByIdAsync(cancellationToken, id);
            if (@event == null)
                throw new Exception("Event not found");
            if (!@event.IsConfirmed)
            {
                throw new Exception("Event Is Already confirmed");
            }
            return @event.Adapt<EventResponseModel>();
        }

        public async Task<EventResponseModel> GetUserEventAsync(
          int id,
          CancellationToken cancellationToken,
          string userId)
        {
            Event @event = await this._unitOfWork.Event.GetByIdAsync(cancellationToken, id);
            if (@event == null)
                throw new Exception("Event not found");
            if (@event.UserId != userId)
                throw new Exception("Event does not belongs to the user");
            EventResponseModel userEventAsync = @event.Adapt<EventResponseModel>();
            return userEventAsync;
        }

        public async Task<EventResponseModel> GetUnconfirmedEventForAdminAsync(
          int id,
          CancellationToken cancellationToken)
        {
            Event @event = await _unitOfWork.Event.GetByIdAsync(cancellationToken, id);
            if (@event == null)
                throw new Exception("Event not found");
            if (@event.IsConfirmed)
            {
                throw new Exception("Event is already confirmed");
            }
            return @event.Adapt<EventResponseModel>();
        }
        public async Task UpdateEventAsync(
          EventUpdateRequestModel @event,
          int id,
          string userId,
          CancellationToken cancellationToken)
        {
            Event entity = await _unitOfWork.Event.GetByIdAsync(cancellationToken, id);
            if (entity == null)
                throw new Exception("Event not found");
            if (entity.UserId != userId)
                throw new Exception("Event does not belongs to the user");
            if (entity.ConfirmedAt.AddDays(entity.DaysForUpdate) < DateTime.Now && entity.IsConfirmed)
                throw new Exception("Update date exceeded");
            entity.Title = @event.Title;
            entity.Description = @event.Description;
            entity.ImageUrl = @event.ImageUrl;
            entity.UpdatedAt = DateTime.Now;
            _unitOfWork.Event.Update(cancellationToken, entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task ConfirmEvent(int id, int bookTimeInMinutes, int daysForUpdate, CancellationToken cancellationToken = default)
        {
            var @event = await _unitOfWork.Event.GetByIdAsync(cancellationToken, id);
            if (@event is null)
            {
                throw new Exception("Event not found");
            }
            if (@event.IsConfirmed)
            {
                throw new Exception("Event is already confirmed");
            }
            @event.BookTimeInMinutes = bookTimeInMinutes;
            @event.DaysForUpdate = daysForUpdate;
            @event.IsConfirmed = true;
            @event.ConfirmedAt = DateTime.Now;
            _unitOfWork.Event.Update(cancellationToken, @event);
            await _unitOfWork.SaveAsync();
        }
        public async Task MoveEventsToArchive(CancellationToken cancellationToken)
        {
            List<Event> finishedEvents = await _unitOfWork.Event.GetAllFinishedEvents(cancellationToken);
            List<ArchivedEventRequestModel> adapted = finishedEvents.Adapt<List<ArchivedEventRequestModel>>();
            List<ArchivedEvent> entities = adapted.Adapt<List<ArchivedEvent>>();
            await _unitOfWork.ArchivedEvent.AddRange(entities, cancellationToken);
            _unitOfWork.Event.RemoveRange(finishedEvents);
            await _unitOfWork.SaveAsync();
        }
    }
}
