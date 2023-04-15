using Domain.Events;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure._ArchivedEvent
{
    public class ArchivedEventRepository:BaseRepository<ArchivedEvent>, IArchivedEventRepository
    {
        private readonly ApplicationDbContext db;

        public ArchivedEventRepository(ApplicationDbContext db)
          : base(db)
        {
            this.db = db;
        }

        public async Task AddRange(List<ArchivedEvent> events, CancellationToken cancellationToken)
        {
            if (events == null)
                return;
            await db.AddRangeAsync(events, cancellationToken);
        }
    }
}
