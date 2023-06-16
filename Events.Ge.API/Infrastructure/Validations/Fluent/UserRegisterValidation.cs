using Application.User.Model;
using FluentValidation;

namespace Events.Ge.API.Infrastructure.Validations.Fluent
{
    public class UserRegisterValidation : AbstractValidator<UserRegisterModel>
    {
        public UserRegisterValidation()
        {
            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(4)
                .MaximumLength(30)
                .Matches("^[A-Za-z0-9]*$");
            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(25)
                .Matches("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$");
        }
    }
}
