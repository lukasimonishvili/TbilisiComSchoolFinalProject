using System;

namespace Infrastructure.Exceptions
{
    public class LoanStatusException : Exception
    {
        public LoanStatusException() : base("Loan status must be 'Pending' to do this action")
        {

        }
    }
}
