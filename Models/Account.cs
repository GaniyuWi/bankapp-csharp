using System;

namespace Bank_API_EF.Models
{
    public enum AccountType
    {
        Savings,
        Current
    }

    public class Account
    {
        public int Id { get; set; }
        public long AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public required string BVN { get; set; }
        public required string PhoneNumber { get; set; }
        public required string NIN { get; set; }
        public AccountType AccountType { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public required string PinHash { get; set; } 
        public bool IsDeleted { get; set; }

    }
}