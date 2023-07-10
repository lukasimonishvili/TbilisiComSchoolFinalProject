using Domain.DTO.Loan;
using Domain.Entities;
using FluentValidation;
using System.Linq;

namespace Application.Accountant.Validators
{
    public class LoanAccountantValidator : AbstractValidator<LoanAccountantDTO>
    {
        public LoanAccountantValidator()
        {
            RuleFor(x => x.Type)
                .NotNull().WithMessage("Loan type is required field and it must be filled")
                .NotEmpty().WithMessage("Loan type is empty")
                .Must(IsTypeCorrect).WithMessage("Unknwn loan type");

            RuleFor(x => x.Amount)
                .NotNull().WithMessage("Amount is required field and it must be filled")
                .GreaterThan(0).WithMessage("Loan amount must be greater then 0");

            RuleFor(x => x.Curency)
                .NotNull().WithMessage("Curency type is required field and it must be filled")
                .NotEmpty().WithMessage("Curency type is empty");

            RuleFor(x => x.LoanMonths)
                .NotNull().WithMessage("LoanMonths is required field and it must be filled")
                .GreaterThan(0).WithMessage("LoanMonths must be greater then 0");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Loan status is required field and it must be filled")
                .NotEmpty().WithMessage("Loan status is empty")
                .Must(IsStatusCorrect).WithMessage("Unknwn loan status");
        }

        private bool IsTypeCorrect(string type)
        {
            string[] loanTypes = { LoanTypes.Auto, LoanTypes.Quick, LoanTypes.Installment };
            return loanTypes.Contains(type);
        }

        private bool IsStatusCorrect(string status)
        {
            string[] statusTypes = { LoanStatuses.Denied, LoanStatuses.Pending, LoanStatuses.Accepted };
            return statusTypes.Contains(status);
        }
    }
}
