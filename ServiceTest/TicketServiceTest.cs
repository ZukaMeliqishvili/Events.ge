using Application._Event;
using Application._Ticket;
using Domain._Ticket;
using Domain.Events;
using Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net.Sockets;

namespace ServiceTest
{
    public class TicketServiceTest
    {
        private readonly string _userId = new Guid().ToString();
        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork> { DefaultValue = DefaultValue.Empty };
        private readonly TicketService _ticketService;
        private readonly CancellationToken cancellationToken = new CancellationToken();
        public TicketServiceTest()
        {
            _ticketService = new TicketService(_unitOfWork.Object);
        }
        private Event GetEventDomain()
        {
            return new Event
            {
                Id = 1,
                Title = "Some Event",
                Description = "Come and enjoy",
                StartDate = DateTime.Now.AddHours(1),
                EndDate = DateTime.Now.AddHours(3),
                NumberOfTickets = 1000,
                TicketPrice = 35,
                ImageUrl = "",
                ConfirmedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsConfirmed = true,
                DaysForUpdate = 1,
                BookTimeInMinutes = 1,
                UserId = _userId
            };
        }

        //book ticket
        [Fact]
        public async Task WhenUserDoesNotHaveBookInTableShouldNotThrowException()
        {
            var @event = GetEventDomain();
            int ticketsCount = @event.NumberOfTickets;
            var entity = new Ticket
            {

                UserId = _userId,
                EventId = @event.Id,
                BookedTill = DateTime.Now.AddMinutes(@event.BookTimeInMinutes)
            };
            _unitOfWork.Setup(x => x.Ticket.GetByUserAndEventIdAsync(It.IsAny<string>(),It.IsAny<int>(),cancellationToken));
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            _unitOfWork.Setup(x => x.Ticket.CreateAsync(cancellationToken, entity));
            _unitOfWork.Setup(x => x.Event.Update(cancellationToken, @event));
            await _ticketService.BookTicket(@event.Id, _userId, cancellationToken);
            Assert.Equal(++@event.NumberOfTickets, ticketsCount);
        }
        [Fact]
        public async Task WhenUserAlreadyBookedTicketShouldThrowException()
        {
            var ticket = new Ticket()
            {
                UserId = _userId,
                EventId = 1,
                BookedTill = DateTime.Now.AddMinutes(3)
            };
            var @event = GetEventDomain();
            _unitOfWork.Setup(x=>x.Event.GetByIdAsync(cancellationToken,It.IsAny<int>())).ReturnsAsync(@event);
            _unitOfWork.Setup(x => x.Ticket.GetByUserAndEventIdAsync(_userId, @event.Id, cancellationToken))
                .ReturnsAsync(ticket);
            var task = async () => await _ticketService.BookTicket(@event.Id, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Ticket To This Event is already booked", ex.Message);
        }
        [Fact]
        public async Task WhenWhileBookingEventDoesNotExsitsShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, It.IsAny<int>()));
            var task = async () => await _ticketService.BookTicket(2, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenWhileBookingEventIsNotConfirmedShouldThrowException()
        {
            var @event = GetEventDomain();
            @event.IsConfirmed = false;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            var task = async () => await _ticketService.BookTicket(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenWhileBookingEventAlreadyStartedShouldThrowException()
        {
            var @event = GetEventDomain();
            @event.StartDate = DateTime.Now.AddHours(-1);
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            var task = async () => await _ticketService.BookTicket(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event is already started", ex.Message);
        }
        [Fact]
        public async Task WhenWhileBookingOutOfTicketsShouldThrowException()
        {
            var @event = GetEventDomain();
            @event.NumberOfTickets = 0;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            var task = async () => await _ticketService.BookTicket(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Out of tickets", ex.Message);
        }

        ////buy ticket
        [Fact]
        public async Task WhenTicketWasNotBookedShouldDecraseTicketCount()
        {
            var @event = GetEventDomain();
            int tickets = @event.NumberOfTickets;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            _unitOfWork.Setup(x => x.Ticket.GetByUserAndEventIdAsync(It.IsAny<string>(), It.IsAny<int>(), cancellationToken));
            _unitOfWork.Setup(x => x.Ticket.CreateAsync(cancellationToken, It.IsAny<Ticket>()));
            _unitOfWork.Setup(x => x.SaveAsync());
            await _ticketService.BuyTicket(@event.Id, _userId, cancellationToken);
            Assert.Equal(--tickets, @event.NumberOfTickets);
        }
        [Fact]
        public async Task WhenTicketWasBookedShouldNotDecraseTicketCount()
        {
            var @event = GetEventDomain();
            int tickets = @event.NumberOfTickets;
            var ticket = new Ticket
            {
                Id = 1,
                EventId = @event.Id,
                UserId = _userId,
                BookedTill = DateTime.Now,
                IsBought = false
            };
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            _unitOfWork.Setup(x => x.Ticket.GetByUserAndEventIdAsync(_userId, @event.Id, cancellationToken))
                .ReturnsAsync(ticket);
            _unitOfWork.Setup(x => x.Ticket.Update(cancellationToken, ticket));
            _unitOfWork.Setup(x => x.SaveAsync());
            await _ticketService.BuyTicket(@event.Id, _userId, cancellationToken);
            bool b = (tickets == @event.NumberOfTickets) && (ticket.IsBought == true);
            Assert.True(b);
        }
        [Fact]
        public async Task WhenWhileBuyingEventNotFoundShouldThrowException()
        {
            var @event = GetEventDomain();
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, It.IsAny<int>()));
            var task = async () => await _ticketService.BuyTicket(@event.Id, _userId, cancellationToken);
            var ex =await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
            
        }
        [Fact]
        public async Task WhenEventIsNotConfirmedShouldThrowException()
        {
            var @event = GetEventDomain();
            @event.IsConfirmed = false;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            var task = async () => await _ticketService.BuyTicket(@event.Id, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found",ex.Message);
        }
        [Fact]
        public async Task WhenEventIsAlreadyStartedShouldThrowException()
        {
            var @event = GetEventDomain();
            @event.StartDate = DateTime.Now.AddHours(-1);
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            var task = async () => await _ticketService.BuyTicket(@event.Id, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event is already started",ex.Message);
        }
        [Fact]
        public async Task WhenOutOfTicketsShouldThrowException()
        {
            var @event = GetEventDomain();
            @event.NumberOfTickets = 0;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            _unitOfWork.Setup(x => x.Ticket.GetByUserAndEventIdAsync(_userId, @event.Id, cancellationToken))
                .ReturnsAsync(It.IsAny<Ticket>());
            var task = async () => await _ticketService.BuyTicket(@event.Id, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("out of tickets", ex.Message);
        }
        //delete booked ticket
        [Fact]
        public async Task WhenBookIsDeletedShouldNotThrowException()
        {
            var ticket = new Ticket()
            {
                Id = 1,
                EventId = 1,
                UserId = _userId,
                BookedTill = DateTime.Now,
                IsBought = false
            };
            _unitOfWork.Setup(x=>x.Ticket.GetByIdAsync(cancellationToken,1)).ReturnsAsync(ticket);
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, It.IsAny<int>())).ReturnsAsync(GetEventDomain());
            _unitOfWork.Setup(x => x.Ticket.DeleteAsync(cancellationToken, 1));
            _unitOfWork.Setup(x => x.SaveAsync());
            var task = async () => await _ticketService.DeleteAsync(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Null(ex);
        }
        [Fact]
        public async Task WhenTicketNotFoundShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Ticket.GetByIdAsync(cancellationToken, It.IsAny<int>()));
            var task = async () => await _ticketService.DeleteAsync(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Ticket not found",ex.Message);
        }
        [Fact]
        public async Task WhenTicketDoesNotBelongToUserShouldThrowException()
        {
            var ticket = new Ticket()
            {
                Id = 1,
                EventId = 1,
                UserId = _userId,
                BookedTill = DateTime.Now,
                IsBought = false
            };
            _unitOfWork.Setup(x => x.Ticket.GetByIdAsync(cancellationToken, It.IsAny<int>())).ReturnsAsync(ticket);
            var task = async () => await _ticketService.DeleteAsync(1, "blabal", cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("There was some problem while deleting your ticket", ex.Message);
        }
        [Fact]
        public async Task WhenTicketIsBoughtShouldThrowException()
        {
            var ticket = new Ticket()
            {
                Id = 1,
                EventId = 1,
                UserId = _userId,
                BookedTill = DateTime.Now,
                IsBought = true
            };
            _unitOfWork.Setup(x => x.Ticket.GetByIdAsync(cancellationToken, It.IsAny<int>())).ReturnsAsync(ticket);
            var task = async () => await _ticketService.DeleteAsync(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("You cant remove ticket which is already bought", ex.Message);
        }
        //buy booked
        [Fact]
        public async Task WhenBookedTicketBoughtShouldNotThrowException()
        {
            var ticket = new Ticket()
            {
                Id = 1,
                EventId = 1,
                UserId = _userId,
                BookedTill = DateTime.Now,
                IsBought = false
            };
            _unitOfWork.Setup(x => x.Ticket.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(ticket);
            _unitOfWork.Setup(x => x.SaveAsync());
            await _ticketService.BuyBooked(1, _userId, cancellationToken);
            Assert.True(ticket.IsBought);
        }
        [Fact] 
        public async Task WhenBookedTicketNotFoundShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Ticket.GetByIdAsync(cancellationToken, It.IsAny<int>()));
            var task = async () => await _ticketService.BuyBooked(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Ticket not found", ex.Message);
        }
        [Fact]
        public async Task WhenBookedTicketDoesNotBelongTouserShouldThrowException()
        {
            var ticket = new Ticket()
            {
                Id = 1,
                EventId = 1,
                UserId = _userId,
                BookedTill = DateTime.Now,
                IsBought = false
            };
            _unitOfWork.Setup(x => x.Ticket.GetByIdAsync(cancellationToken, It.IsAny<int>())).ReturnsAsync(ticket);
            var task = async () => await _ticketService.BuyBooked(1, "1", cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("There has been some problem while buying ticket", ex.Message);
        }
        [Fact]
        public async Task WhenTicketisAlreadyBoughtShouldThrowException()
        {
            var ticket = new Ticket()
            {
                Id = 1,
                EventId = 1,
                UserId = _userId,
                BookedTill = DateTime.Now,
                IsBought = true
            };
            _unitOfWork.Setup(x => x.Ticket.GetByIdAsync(cancellationToken, It.IsAny<int>())).ReturnsAsync(ticket);
            var task = async () => await _ticketService.BuyBooked(1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Ticket is already bought", ex.Message);
        }
    }
}
