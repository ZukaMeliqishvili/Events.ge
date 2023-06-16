using Application._Event.Model.Request;
using FluentValidation;

namespace Events.Ge.API.Infrastructure.Validations.Fluent
{
    public class EventRequestModelValidation : AbstractValidator<EventRequestModel>
    {
        public EventRequestModelValidation()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(50);
            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(200);
            RuleFor(x => x.StartDate)
                .NotNull()
                .Must(x => x > DateTime.Now);
            RuleFor(x => x.EndDate)
                .NotNull()
                .Must(x => x > DateTime.Now);
            RuleFor(x => x.NumberOfTickets)
                .Must(x => x > 0);
            RuleFor(x => x.TicketPrice)
                .Must(x => x >= 1);

        }
    }
}
