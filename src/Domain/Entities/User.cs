using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsBlocked { get; set; }
        public bool Verified { get; set; }
        public string Role { get; set; }
        public List<Loan> Loans { get; set; }
    }

    public static class Roles
    {
        public static string Accountant = "Accountant";
        public static string User = "User";
    }
}
