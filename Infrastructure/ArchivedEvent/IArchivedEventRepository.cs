using Domain.Events;

namespace Infrastructure._ArchivedEvent
{
    public interface IArchivedEventRepository:IBaseRepository<ArchivedEvent>
    {
        Task AddRange(List<ArchivedEvent> events, CancellationToken cancellationToken);
    }
}
