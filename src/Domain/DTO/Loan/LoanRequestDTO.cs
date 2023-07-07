namespace Domain.DTO.Loan
{
    public class LoanRequestDTO
    {
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Curency { get; set; }
        public int LoanMonths { get; set; }
    }
}
