// File: Services/AccountService.cs
using Bank_API_EF.Dtos;
using Bank_API_EF.Models;
using BankApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Bank_API_EF.Data;

namespace BankApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;
        private readonly BankContext _context; 
        private const decimal MinimumInitialBalance = 1000m;

        public AccountService(IAccountRepository repo, BankContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<AccountResponseDto> CreateAccountAsync(CreateAccountDto dto)
        {
            if (dto.Balance < MinimumInitialBalance)
                throw new ArgumentException($"Initial balance must be at least {MinimumInitialBalance}");

            // generate unique 10-digit account number
            long accountNumber = await GenerateUniqueAccountNumberAsync();

            var account = new Account
            {
                AccountNumber = accountNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                BVN = dto.BVN,
                NIN = dto.NIN,
                AccountType = dto.AccountType,
                Balance = Math.Round(dto.Balance, 2),
                PinHash = BCrypt.Net.BCrypt.HashPassword(dto.Pin), // hash the PIN
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                IsDeleted = false
            };

            await _repo.AddAsync(account);
            await _repo.SaveChangesAsync();

            return MapToDto(account);
        }




        public async Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync()
        {
            var accounts = await _repo.GetAllAsync();
            return accounts.Select(MapToDto);
        }
        public async Task<AccountResponseDto?> GetByAccountNumberAsync(long accountNumber)
        {
            var account = await _repo.GetByAccountNumberAsync(accountNumber);
            return account == null ? null : MapToDto(account);
        }
        public async Task<AccountResponseDto?> UpdateAccountAsync(long accountNumber, AccountUpdateDto dto)
        {
            var account = await _repo.GetByAccountNumberAsync(accountNumber);
            if (account == null) return null;

            // Do not allow update of AccountNumber or NIN here
            account.FirstName = dto.FirstName;
            account.LastName = dto.LastName;
            account.Email = dto.Email;
            account.Address = dto.Address;
            account.PhoneNumber = dto.PhoneNumber;
            account.AccountType = dto.AccountType;

            await _repo.UpdateAsync(account);
            await _repo.SaveChangesAsync();

            return MapToDto(account);
        }





        public async Task<bool> DepositAsync(DepositDto dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("Deposit amount must be positive");

            var account = await _repo.GetByAccountNumberAsync(dto.AccountNumber);
            if (account == null) return false;

            account.Balance = Math.Round(account.Balance + dto.Amount, 2);
            await _repo.UpdateAsync(account);

            _context.Transactions.Add(new Transaction
            {
                AccountNumber = account.AccountNumber,
                Type = TransactionType.Deposit,
                Amount = dto.Amount,
                Balance = account.Balance,
                Description = "Cash Deposit"
            });
            await _repo.SaveChangesAsync();
            return true;
        }




        public async Task<bool> WithdrawAsync(WithdrawDto dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("Withdrawal amount must be positive");

            var account = await _repo.GetByAccountNumberAsync(dto.AccountNumber);
            if (account == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.PinHash, account.PinHash)) return false;

            if (account.Balance < dto.Amount) return false; // insufficient funds

            account.Balance = Math.Round(account.Balance - dto.Amount, 2);
            await _repo.UpdateAsync(account);

            _context.Transactions.Add(new Transaction
            {
                AccountNumber = account.AccountNumber,
                Type = TransactionType.Withdrawal,
                Amount = dto.Amount,
                Balance = account.Balance,
                Description = "Cash Withdrawal"
            });


            await _repo.SaveChangesAsync();
            return true;
        }



        public async Task<bool> TransferAsync(TransferDto dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("Transfer amount must be positive");

            var fromAccount = await _repo.GetByAccountNumberAsync(dto.FromAccountNumber);
            var toAccount = await _repo.GetByAccountNumberAsync(dto.ToAccountNumber);

            if (fromAccount == null || toAccount == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.FromPin, fromAccount.PinHash)) return false;

            if (fromAccount.Balance < dto.Amount) return false; // insufficient funds

            // Use DB transaction to ensure consistency
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                fromAccount.Balance = Math.Round(fromAccount.Balance - dto.Amount, 2);
                toAccount.Balance = Math.Round(toAccount.Balance + dto.Amount, 2);

                await _repo.UpdateAsync(fromAccount);
                await _repo.UpdateAsync(toAccount);

                _context.Transactions.Add(new Transaction
                {
                    AccountNumber = dto.FromAccountNumber, // corrected property name
                    Type = TransactionType.TransferOut,
                    Amount = dto.Amount,
                    Balance = fromAccount.Balance,
                    Description = $"Transfered to {toAccount.FirstName} {toAccount.LastName} ({toAccount.AccountNumber})"
                });

                _context.Transactions.Add(new Transaction
                {
                    AccountNumber = dto.ToAccountNumber, // corrected property name
                    Type = TransactionType.TransferIn,
                    Amount = dto.Amount,
                    Balance = fromAccount.Balance,
                    Description = $"Transfered from {fromAccount.FirstName} {fromAccount.LastName} {fromAccount.AccountNumber}"
                });
                await _repo.SaveChangesAsync();

                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> DeleteAccountAsync(long accountNumber, string pin)
        {
            var account = await _repo.GetByAccountNumberAsync(accountNumber);
            if (account == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(pin, account.PinHash)) return false;

            // Soft delete by setting IsDeleted flag
            account.IsDeleted = true;
            await _repo.UpdateAsync(account);
            // If you want to hard delete, uncomment the next line and comment out the above lines
            // await _repo.DeleteAsync(account);
             await _repo.SaveChangesAsync();
            return true;
        }



        public async Task<bool> UpdatePinAsync(UpdatePinDto dto)
        {
            var account = await _repo.GetByAccountNumberAsync(dto.AccountNumber);
            if (account == null) return false;

            // Verify old PIN
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPin, account.PinHash))
                return false;

            // Hash new PIN
            account.PinHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPin);

            await _repo.UpdateAsync(account);
            await _repo.SaveChangesAsync();
            return true;
        }


        private AccountResponseDto MapToDto(Account account)
        {

            return new AccountResponseDto(
            account.AccountNumber,
            account.FirstName,
            account.LastName,
            account.PhoneNumber,
            account.Email,
            account.Address,
            account.BVN,
            account.NIN,
            account.AccountType,
            account.Balance,
            account.CreatedAt
            );
        }


        private async Task<long> GenerateUniqueAccountNumberAsync()
        {
            long candidate;
            var random = new Random();

            do
            {
                candidate = long.Parse(string.Concat(Enumerable.Range(0, 10).Select(_ => random.Next(0, 10))));
            }
            while (await _repo.GetByAccountNumberAsync(candidate) != null);

            return candidate;
        }


        public async Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(long accountNumber)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountNumber == accountNumber)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return transactions.Select(t => new TransactionDto(t.Type, t.Amount, t.Date, t.Balance, t.Description));
        }


        private int GenerateRandomDigits(int length)
        {
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int digit = RandomNumberGenerator.GetInt32(0, 10); 
                                                                 
                sb.Append(digit);
            }
            return int.Parse(sb.ToString());
        }


    }
}