using BCryptNet = BCrypt.Net.BCrypt;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence
{
    public class DataBaseContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Role = Roles.Accountant,
                    Username = "admin",
                    Password = BCryptNet.HashPassword("admin"),
                    Email = "admin@testmail.com",
                    BirthDate = DateTime.Now,
                    IsBlocked = false,
                    Verified = true
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=LoanDB;Trusted_Connection=True;MultipleActiveResultSets=True"
            );
        }
    }
}
