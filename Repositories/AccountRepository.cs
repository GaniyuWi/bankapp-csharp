using Bank_API_EF.Data;
using Bank_API_EF.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BankApp.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankContext _context;


        public AccountRepository(BankContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }


        public async Task<Account?> GetByAccountNumberAsync(long accountNumber)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && !a.IsDeleted);
        }


        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts.Where(a => !a.IsDeleted).ToListAsync();
        }


        public Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            return Task.CompletedTask;
        }


        public Task DeleteAsync(Account account)
        {
            _context.Accounts.Remove(account);
            return Task.CompletedTask;
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}