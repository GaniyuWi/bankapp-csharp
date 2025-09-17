using Bank_API_EF.Models;

namespace Bank_API_EF.Dtos;

public class AccountUpdateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public AccountType AccountType { get; set; }
    // NIN and AccountNumber are intentionally excluded from update
}
