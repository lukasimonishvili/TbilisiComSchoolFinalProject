using Domain.DTO.Loan;
using System.Collections.Generic;

namespace Domain.Interface
{
    public interface ILoanService
    {
        string RequestLoan(LoanRequestDTO loanRequestDto, string authorizationHeader);
        List<LoanDTO> GetLoansByUserId(int userId, string authorizationHeader);
        string DeleteLoanById(int loanId, string authorizationHeader);
        string UpdateLoan(int loanId, LoanRequestDTO loanReque, string authorizationHeader);
    }
}
