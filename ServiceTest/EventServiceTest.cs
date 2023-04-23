using Application._Event;
using Application._Event.Model.Request;
using Application._Ticket;
using Domain.Events;
using Infrastructure;
using Mapster;

namespace ServiceTest
{
    public class EventServiceTest
    {
        private readonly string _userId = new Guid().ToString();
        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork> { DefaultValue = DefaultValue.Empty };
        private readonly EventService _eventService;
        private readonly CancellationToken cancellationToken = new CancellationToken();
        private readonly Mock<ITicketService> _ticketService;
        public EventServiceTest()
        {
            var ticketService = new TicketService(_unitOfWork.Object);

            _eventService = new EventService(_unitOfWork.Object, ticketService);
        }
        private EventRequestModel GetEventRequestModel()
        {
            EventRequestModel eventRequest = new EventRequestModel()
            {
                Title = "Some Event",
                Description = "Come and enjoy",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(3),
                NumberOfTickets = 1000,
                TicketPrice = 35,
                ImageUrl = "",
            };
            return eventRequest;
        }
        private EventUpdateRequestModel GetUpdateModel()
        {
            return new EventUpdateRequestModel
            {
                Title = "New Title",
                Description = "New Description",
                ImageUrl = ""
            };
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
        private List<Event> GetFinishedEvents()
        {
            var list = new List<Event>
            {
                new Event
                {
                         Id = 1,
                    Title = "Some Event",
                    Description = "Come and enjoy",
                    StartDate = DateTime.Now.AddHours(-3),
                    EndDate = DateTime.Now.AddHours(-1),
                    NumberOfTickets = 1000,
                    TicketPrice = 35,
                    ImageUrl = "",
                    ConfirmedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsConfirmed = true,
                    DaysForUpdate = 1,
                    BookTimeInMinutes = 1,
                    UserId = _userId
                },
                new Event
                {
                        Id = 2,
                    Title = "Some Event",
                    Description = "Come and enjoy",
                    StartDate = DateTime.Now.AddHours(-3),
                    EndDate = DateTime.Now.AddHours(-1),
                    NumberOfTickets = 1000,
                    TicketPrice = 35,
                    ImageUrl = "",
                    ConfirmedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsConfirmed = true,
                    DaysForUpdate = 1,
                    BookTimeInMinutes = 1,
                    UserId = _userId
                }
            };
            return list;
        }
        [Fact]
        public async Task WhenEventIsCreatedShouldNotThrowException()
        {

            _unitOfWork.Setup(x => x.Event.CreateAsync(cancellationToken, It.IsAny<Event>()));

            var task = async () => await _eventService.CreateAsync(GetEventRequestModel(), It.IsAny<string>(), cancellationToken);
            var test = await Record.ExceptionAsync(task);


            Assert.Null(test);
        }
        [Fact]
        public async Task WhenRequestModelIsNullShouldThrowArgumentNullException()
        {

            _unitOfWork.Setup(x => x.Event.CreateAsync(cancellationToken, It.IsAny<Event>()));

            var task = async () => await _eventService.CreateAsync(null, It.IsAny<string>(), cancellationToken);


            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }
        [Fact]
        public async Task WhenStartDateIsHigherThanEndDateShouldThrowInvalidRequestException()
        {

            _unitOfWork.Setup(x => x.Event.CreateAsync(cancellationToken, It.IsAny<Event>()));
            var requestModel = GetEventRequestModel();
            requestModel.StartDate = DateTime.Now;
            requestModel.EndDate = DateTime.Now.AddHours(-1);
            var task = async () => await _eventService.CreateAsync(requestModel, It.IsAny<string>(), cancellationToken);
            var exception = await  Record.ExceptionAsync(task);
            Assert.Equal("Event startd date must be higher than end date", exception.Message);
        }
        //eventUpdate
        [Fact]
        public async Task WhenEventIsUpdatedShoulNotThrowExeption()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            _unitOfWork.Setup(x => x.Event.Update(cancellationToken, GetEventDomain()));
            var task = async () => await _eventService.UpdateEventAsync(GetUpdateModel(), 1, _userId, cancellationToken);
            var test = await Record.ExceptionAsync(task);
            Assert.Null(test);
        }
        [Fact]
        public async Task WhenEntityByIdNotFoundShoudThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            _unitOfWork.Setup(x => x.Event.Update(cancellationToken, GetEventDomain()));
            var task = async () => await _eventService.UpdateEventAsync(GetUpdateModel(), 3, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenEventDoesNotBelongToUserShoudThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            _unitOfWork.Setup(x => x.Event.Update(cancellationToken, GetEventDomain()));
            var task = async () => await _eventService.UpdateEventAsync(GetUpdateModel(), 1, "blablalba", cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event does not belongs to the user",ex.Message);
        }
        [Fact]
        public async Task WhenUpdateDateIsExceededShoudThrowException()
        {
            var entity = GetEventDomain();
            entity.ConfirmedAt = DateTime.Now.AddDays(-5);
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(entity);
            _unitOfWork.Setup(x => x.Event.Update(cancellationToken, GetEventDomain()));
            var task = async () => await _eventService.UpdateEventAsync(GetUpdateModel(), 1, _userId, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Update date exceeded", ex.Message);
        }
        //Delete
        [Fact]
        public async Task WhenEventIsDeletedShouldNotThrowException()
        {
            var entity = GetEventDomain();
            entity.IsConfirmed = false;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(entity);
            _unitOfWork.Setup(x => x.Event.DeleteAsync(cancellationToken, 1));
            var task = async () => await _eventService.Delete(cancellationToken, 1);
            var test = await Record.ExceptionAsync(task);
            Assert.Null(test);
        }
        [Fact]
        public async Task WhenEventNotFoundShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, It.IsAny<int>())).ReturnsAsync(It.IsAny<Event>());
            _unitOfWork.Setup(x => x.Event.DeleteAsync(cancellationToken, It.IsAny<int>()));
            var task = async () => await _eventService.Delete(cancellationToken, It.IsAny<int>());
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenEventIsConfirmedShoudThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            _unitOfWork.Setup(x => x.Event.DeleteAsync(cancellationToken, 1));
            var task = async () => await _eventService.Delete(cancellationToken, 1);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event is already confirmed", ex.Message);
        }

        //GetConfirmedEventById
        [Fact]
        public async Task WhenConfirmedEventIsReturnedShoudNotThrowException()
        {
            var @event = GetEventDomain();
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            var model = await _eventService.GetConfirmedEventByIdAsync(1, cancellationToken);
            Assert.Equal(@event.Id, model.Id);
        }
        [Fact]
        public async Task WhenConfirmedEventNotFoundShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            var task = async () => await _eventService.GetConfirmedEventByIdAsync(2, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenEventIsNotConfirmedShoudThrowException()
        {
            var entity = GetEventDomain();
            entity.IsConfirmed = false;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(entity);
            var task = async () => await _eventService.GetConfirmedEventByIdAsync(1, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event Is Already confirmed", ex.Message);
        }
        //getUserEvent
        [Fact]
        public async Task WhenUserEventIsReturnedShouldNotThrowException()
        {
            var @event = GetEventDomain();
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            var model = await _eventService.GetUserEventAsync(1, cancellationToken, _userId);
            Assert.Equal(@event.Id, model.Id);
        }
        [Fact]
        public async Task WhenUserEventNotFoundShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            var task = async () => await _eventService.GetUserEventAsync(2, cancellationToken, _userId);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenEventDoesNotBelongToUserShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            var task = async () => await _eventService.GetUserEventAsync(1, cancellationToken, "blabla");
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event does not belongs to the user", ex.Message);
        }
        //GetUnconfirmedEventForAdmin
        [Fact]
        public async Task WhenUnconfirmedEventIsReturnedShouldNotThrowException()
        {
            var entity = GetEventDomain();
            entity.IsConfirmed = false;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(entity);
            var model = await _eventService.GetUnconfirmedEventForAdminAsync(1, cancellationToken);
            Assert.Equal(entity.Id, model.Id);
        }
        [Fact]
        public async Task WhenEventForAdminNotFoundShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, It.IsAny<int>()));
            var task = async () => await _eventService.GetUnconfirmedEventForAdminAsync(1, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenEventIsConfimedShoudThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            var task = async () => await _eventService.GetUnconfirmedEventForAdminAsync(1, cancellationToken);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event is already confirmed", ex.Message);
        }
        //confirm event
        public async Task WhenEverythingIsAllrightIsConfirmedMustBeTrue()
        {
            var @event = GetEventDomain();
            @event.IsConfirmed = false;
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(@event);
            int daysforupdate = 10;
            int booktimeinminutes = 5;
            await _eventService.ConfirmEvent(@event.Id, booktimeinminutes, daysforupdate);
            Assert.Equal(booktimeinminutes, @event.BookTimeInMinutes);
            Assert.Equal(daysforupdate, @event.DaysForUpdate);
            Assert.True(@event.IsConfirmed);
        }
        [Fact]
        public async Task WhenEventIsNullWhileConfrimationShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, It.IsAny<int>()));
            int daysforupdate = 10;
            int booktimeinminutes = 5;
            var task = async () => await _eventService.ConfirmEvent(111, booktimeinminutes, daysforupdate);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event not found", ex.Message);
        }
        [Fact]
        public async Task WhenEventIsActiveWhileConfrimationShouldThrowException()
        {
            _unitOfWork.Setup(x => x.Event.GetByIdAsync(cancellationToken, 1)).ReturnsAsync(GetEventDomain());
            int daysforupdate = 10;
            int booktimeinminutes = 5;
            var task = async () => await _eventService.ConfirmEvent(1, booktimeinminutes, daysforupdate);
            var ex = await Record.ExceptionAsync(task);
            Assert.Equal("Event is already confirmed", ex.Message);
        }
        //archiver
        [Fact]
        public async Task WhenFinishedEventsAreArchivedShouldNotThrowException()
        {
            var list = GetFinishedEvents();
            _unitOfWork.Setup(x => x.Event.GetAllFinishedEvents(cancellationToken))
                .ReturnsAsync(list);
            var adapted = list.Adapt<List<ArchivedEventRequestModel>>();
            _unitOfWork.Setup(x => x.ArchivedEvent.AddRange(It.IsAny<List<ArchivedEvent>>(), cancellationToken));
            _unitOfWork.Setup(x => x.Event.RemoveRange(list));
            _unitOfWork.Setup(x => x.SaveAsync());

            var task = async () => await _eventService.MoveEventsToArchive(cancellationToken);
            var record = await Record.ExceptionAsync(task);
            Assert.Null(record);
        }
    }
}