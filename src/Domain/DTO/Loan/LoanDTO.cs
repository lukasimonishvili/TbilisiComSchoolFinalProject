namespace Domain.DTO.Loan
{
    public class LoanDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Curency { get; set; }
        public int LoanMonths { get; set; }
        public string Status { get; set; }
    }
}
