using Domain.DTO.Loan;
using System.Collections.Generic;

namespace Domain.Interface
{
    public interface ILoanService
    {
        string RequestLoan(LoanRequestDTO loanRequestDto, string authorizationHeader, bool IsTest = false);
        List<LoanDTO> GetLoansByUserId(int userId, string authorizationHeader, bool IsTest = false);
        string DeleteLoanById(int loanId, string authorizationHeader, bool IsTest = false);
        string UpdateLoan(int loanId, LoanRequestDTO loanReque, string authorizationHeader, bool IsTest = false);
    }
}
