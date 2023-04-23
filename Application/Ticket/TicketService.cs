using Domain._Ticket;
using Domain.Events;
using Infrastructure;
using Microsoft.Extensions.Logging;

namespace Application._Ticket
{
    public class TicketService:ITicketService
    {

        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task BookTicket(int eventId, string userId, CancellationToken cancellationToken)
        {
            

            Event @event = await _unitOfWork.Event.GetByIdAsync(cancellationToken, eventId);
            if (@event == null)
                throw new Exception("Event not found");
            if (!@event.IsConfirmed)
                throw new Exception("Event not found");
            if (@event.StartDate < DateTime.Now)
                throw new Exception("Event is already started");
            if (@event.NumberOfTickets == 0)
                throw new Exception("Out of tickets");
            var ticket = await _unitOfWork.Ticket.GetByUserAndEventIdAsync(userId, eventId, cancellationToken);
            if (ticket != null)
            {
                throw new Exception("Ticket To This Event is already booked");
            }
            var obj = new Ticket();
            obj.EventId = eventId;
            obj.UserId = userId;
            obj.BookedTill = DateTime.Now.AddMinutes(@event.BookTimeInMinutes);
            --@event.NumberOfTickets;
            await _unitOfWork.Ticket.CreateAsync(cancellationToken, obj);
            _unitOfWork.Event.Update(cancellationToken, @event);
            await _unitOfWork.SaveAsync();
        }

        public async Task BuyTicket(int id, string userId, CancellationToken cancellationToken)
        {
            var @event = await _unitOfWork.Event.GetByIdAsync(cancellationToken, id);
            if(@event is null)
            {
                throw new Exception("Event not found");
            }
            if(@event.IsConfirmed==false)
            {
                throw new Exception("Event not found");
            }
            if(@event.StartDate < DateTime.Now)
            {
                throw new Exception("Event is already started");
            }
            var ticket = await _unitOfWork.Ticket.GetByUserAndEventIdAsync(userId,id, cancellationToken);
            if (ticket != null)
            {
                 ticket.IsBought = true;
                 _unitOfWork.Ticket.Update(cancellationToken, ticket);
                 await _unitOfWork.SaveAsync();
                 return;
            }
            if(@event.NumberOfTickets==0)
            {
                throw new Exception("out of tickets");
            }
            var newTicket = new Ticket();
            newTicket.EventId = id;
            newTicket.UserId = userId;
            newTicket.IsBought = true;
            await _unitOfWork.Ticket.CreateAsync(cancellationToken, newTicket);
            --@event.NumberOfTickets;
            await _unitOfWork.SaveAsync();

        }
        public async Task DeleteAsync(int id,string userId, CancellationToken cancellationToken)
        {
           var ticket = await _unitOfWork.Ticket.GetByIdAsync(cancellationToken, id);
           if(ticket is null)
           {
                throw new Exception("Ticket not found");
           }
           if(ticket.UserId!= userId)
           {
                throw new Exception("There was some problem while deleting your ticket");
           }
           if(ticket.IsBought)
           {
                throw new Exception("You cant remove ticket which is already bought");
           }
            var @event = await _unitOfWork.Event.GetByIdAsync(cancellationToken, ticket.EventId);
            ++@event.NumberOfTickets;

            await _unitOfWork.Ticket.DeleteAsync(cancellationToken, id);
            await _unitOfWork.SaveAsync();
        }
        public async Task BuyBooked(int id,string userId, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.Ticket.GetByIdAsync(cancellationToken, id);
            if (ticket is null)
            {
                throw new Exception("Ticket not found");
            }
            if (ticket.UserId != userId)
            {
                throw new Exception("There has been some problem while buying ticket");
            }
            if(ticket.IsBought)
            {
                throw new Exception("Ticket is already bought");
            }
            ticket.IsBought = true;
            await _unitOfWork.SaveAsync();
        }
        public async Task<List<Ticket>> GetAllAsync(CancellationToken cancellationToken)
        {
            List<Ticket> tickets = await _unitOfWork.Ticket.GetAllAsync(cancellationToken);
            return tickets;
        }
        public async Task<List<Ticket>> GetUserTicketsAsync(CancellationToken cancellationToken,string userId)
        {
            return await _unitOfWork.Ticket.GetUserTickets(cancellationToken, userId);
        }
        public async Task RemoveExpiredBookedTickets(CancellationToken cancellationToken)
        {
            List<Ticket> expiredBookings = await _unitOfWork.Ticket.GetAllExpiredAsync(cancellationToken);
            foreach (var book in expiredBookings)
            {
                await UpdateNumberOfTickets(book, cancellationToken);
            }
            _unitOfWork.Ticket.RemoveRange(expiredBookings);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateNumberOfTickets(Ticket entity, CancellationToken cancellationToken)
        {
            var @event = await _unitOfWork.Event.GetByIdAsync(cancellationToken, entity.EventId);
            ++@event.NumberOfTickets;
        }
    }
}
