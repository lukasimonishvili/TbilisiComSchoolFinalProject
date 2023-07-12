using Domain.DTO.Authentication;
using Domain.DTO.Loan;

namespace Domain.Interface
{
    public interface IAccountantService
    {
        string UpdateUser(int userId, UserAccountantDTO user, string authorizationHeader, bool IsTest = false);
        string UpdateLoan(int loanId, LoanAccountantDTO loanDto, string authorizationHeader, bool IsTest = false);
    }
}
