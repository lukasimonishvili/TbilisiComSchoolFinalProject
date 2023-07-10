using Domain.DTO.Authentication;
using Domain.DTO.Loan;

namespace Domain.Interface
{
    public interface IAccountantService
    {
        string UpdateUser(int userId, UserAccountantDTO user, string authorizationHeader);
        string UpdateLoan(int loanId, LoanAccountantDTO loanDto, string authorizationHeader);
    }
}
