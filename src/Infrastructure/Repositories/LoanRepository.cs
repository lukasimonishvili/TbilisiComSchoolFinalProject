using Domain.Entities;
using Domain.Interface;
using Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly static DataBaseContext _context = new();

        public void AddLoanToDatabase(Loan loan)
        {
            _context.Loans.Add(loan);
            _context.SaveChanges();
        }

        public void DeleteLoan(Loan loan)
        {
            _context.Loans.Remove(loan);
            _context.SaveChanges();
        }

        public Loan GetLoanById(int loanId)
        {
            return _context.Loans.FirstOrDefault(l => l.Id == loanId);
        }

        public List<Loan> GetLoansByUsrId(int userId)
        {
            return _context.Loans.Where(x => x.UserId == userId).ToList();
        }

        public void UpdateLoan(Loan newLoan)
        {
            var oldLoan = _context.Loans.FirstOrDefault(l => l.Id == newLoan.Id);
            _context.Entry(oldLoan).CurrentValues.SetValues(newLoan);
            _context.SaveChanges();
        }
    }
}
