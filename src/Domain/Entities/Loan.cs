namespace Domain.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Curency { get; set; }
        public int LoanMonths { get; set; }
        public string Status { get; set; }
    }

    public static class LoanTypes
    {
        public static string Quick = "Quick";
        public static string Auto = "Auto";
        public static string Installment = "Installment";
    }

    public static class LoanStatuses
    {
        public static string Pending = "Pending";
        public static string Accepted = "Accepted";
        public static string Denied = "Denied";
    }
}
