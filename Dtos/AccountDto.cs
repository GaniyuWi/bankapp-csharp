namespace Bank_API_EF.Dtos;

public record class AccountDto(
int Id,
long AccountNumber,
decimal Balance,
string Email,
string FirstName,
string LastName,
string Address,
long BVN,
int PhoneNumber,
DateOnly CreatedAt
);

