using System;
using System.Security.Cryptography.X509Certificates;
using Bank_API_EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_API_EF.Data
{

    public class BankContext(DbContextOptions<BankContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; } = null!;

        public DbSet<Transaction> Transactions { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();


            // decimal mapping: SQLite treats decimal differently; this ensures precision intent.
            modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasColumnType("decimal(18,2)");


            base.OnModelCreating(modelBuilder);
        }
    }
}