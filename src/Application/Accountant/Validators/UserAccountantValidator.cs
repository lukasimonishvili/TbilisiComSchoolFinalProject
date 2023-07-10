using Domain.DTO.Authentication;
using FluentValidation;

namespace Application.Accountant.Validators
{
    public class UserAccountantValidator : AbstractValidator<UserAccountantDTO>
    {
        public UserAccountantValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("Firstname is required field and it must be filled")
                .NotEmpty().WithMessage("Firstname field is empty")
                .MinimumLength(1).WithMessage("Firstname character length must be in range 1 - 50")
                .MaximumLength(50).WithMessage("Firstname character length must be in range 1 - 50");

            RuleFor(x => x.LastName)
               .NotNull().WithMessage("Lastname is required field and it must be filled")
               .NotEmpty().WithMessage("Lastname field is empty")
               .MinimumLength(1).WithMessage("Lastname character length must be in range 1 - 50")
               .MaximumLength(50).WithMessage("Lastname character length must be in range 1 - 50");

            RuleFor(x => x.BirthDate)
                .NotNull().WithMessage("Birtdate is required field and it must be filled")
                .NotEmpty().WithMessage("Birthdate field is empty");

            RuleFor(x => x.Username)
               .NotNull().WithMessage("Username is required field and it must be filled")
               .NotEmpty().WithMessage("Username field is empty")
               .MinimumLength(1).WithMessage("Username character length must be in range 1 - 20")
               .MaximumLength(20).WithMessage("Username character length must be in range 1 - 20");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required field and it must be filled")
                .NotEmpty().WithMessage("Email field is empty")
                .EmailAddress().WithMessage("Incorrect email format");

        }
    }
}
