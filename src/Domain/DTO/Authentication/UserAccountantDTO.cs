﻿using System;

namespace Domain.DTO.Authentication
{
    public class UserAccountantDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; }
        public bool Verified { get; set; }
    }
}
