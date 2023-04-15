using Application._Event.Model.Request;
using Application._Event.Model.Response;

namespace Application._Event
{
    public interface IEventService
    {
        
        Task ConfirmEvent(int id, int bookTimeInMinutes, int daysForUpdate, CancellationToken cancellationToken = default);
        Task CreateAsync(EventRequestModel @event, string userId, CancellationToken cancellationToken);
        Task Delete(CancellationToken cancellation, int id);
        Task<List<EventResponseModel>> GetAllConfirmedEventsAsync(CancellationToken cancellationToken);
        Task<List<EventResponseModel>> GetAllUnconfirmedEventsAsyn(CancellationToken cancellationToken);
        Task<List<EventResponseModel>> GetAllUserEventAsync(CancellationToken cancellationToken, string userId);
        Task<EventResponseModel> GetConfirmedEventByIdAsync(int id, CancellationToken cancellationToken);
        Task<EventResponseModel> GetUnconfirmedEventForAdminAsync(int id, CancellationToken cancellationToken);
        Task<EventResponseModel> GetUserEventAsync(int id, CancellationToken cancellationToken, string userId);
        Task MoveEventsToArchive(CancellationToken cancellationToken);
        Task UpdateEventAsync(EventUpdateRequestModel @event, int id, string userId, CancellationToken cancellationToken);
    }
}
