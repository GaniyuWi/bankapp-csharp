using Bank_API_EF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BankApp.Repositories
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);
        Task<Account?> GetByAccountNumberAsync(long accountNumber);
        Task<IEnumerable<Account>> GetAllAsync();
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
        Task SaveChangesAsync();
    }
}