using Domain.Events;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Configuration
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(i => i.Title).IsRequired(true).HasMaxLength(100);
            builder.Property(i => i.Description).IsRequired(true).HasMaxLength(1000);
            builder.Property(i => i.StartDate).IsRequired(true);
            builder.Property(i => i.EndDate).IsRequired(true);
            builder.Property(i => i.NumberOfTickets).IsRequired(true);
            builder.Property(i => i.TicketPrice).IsRequired(true);
            builder.Property(i => i.ImageUrl).IsRequired(false);
            builder.HasOne(i => i.User).WithMany(i => i.Events).HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
    public class ArchivedEventConfiguration : IEntityTypeConfiguration<ArchivedEvent>
    {
        public void Configure(EntityTypeBuilder<ArchivedEvent> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(i => i.Title).IsRequired(true).HasMaxLength(100);
            builder.Property(i => i.Description).IsRequired(true).HasMaxLength(1000);
            builder.Property(i => i.StartDate).IsRequired(true);
            builder.Property(i => i.EndDate).IsRequired(true);
            builder.Property(i => i.ImageUrl).IsRequired(false);
            builder.HasOne(i => i.User).WithMany(i => i.ArchivedEvents).HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
