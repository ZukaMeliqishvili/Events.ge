using Application._Event.Model.Request;
using FluentValidation;

namespace Events.Ge.API.Infrastructure.Validations.Fluent
{
    public class EventUpdateRequestModelValidator : AbstractValidator<EventUpdateRequestModel>
    {
        public EventUpdateRequestModelValidator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(30);
            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(200);
        }
    }
}
