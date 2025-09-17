using Bank_API_EF.Dtos;
using Bank_API_EF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BankApp.Services
{
    public interface IAccountService
    {
        Task<AccountResponseDto> CreateAccountAsync(CreateAccountDto dto);
        Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync();
        Task<AccountResponseDto?> GetByAccountNumberAsync(long accountNumber);
        Task<AccountResponseDto?> UpdateAccountAsync(long accountNumber, AccountUpdateDto dto);
        Task<bool> DepositAsync(DepositDto dto);
        Task<bool> WithdrawAsync(WithdrawDto dto);
        Task<bool> TransferAsync(TransferDto dto);
        Task<bool> DeleteAccountAsync(long accountNumber, string pin);

        Task<bool> UpdatePinAsync(UpdatePinDto dto);

        Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(long accountNumber);

    }
}