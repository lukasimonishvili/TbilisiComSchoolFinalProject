using Domain.DTO.Loan;
using Domain.Entities;
using FluentValidation;
using System.Linq;

namespace Application.Loans.Validators
{
    public class LoanRequestValidator : AbstractValidator<LoanRequestDTO>
    {
        public LoanRequestValidator()
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
        }

        private bool IsTypeCorrect(string type)
        {
            string[] loanTypes = { LoanTypes.Auto, LoanTypes.Quick, LoanTypes.Installment };
            return loanTypes.Contains(type);
        }
    }
}
