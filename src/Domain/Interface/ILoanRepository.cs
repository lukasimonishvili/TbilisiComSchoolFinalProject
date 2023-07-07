using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interface
{
    public interface ILoanRepository
    {
        void AddLoanToDatabase(Loan loan);
        List<Loan> GetLoansByUsrId(int userId);
        void DeleteLoan(Loan loan);
        Loan GetLoanById(int loanId);
    }
}