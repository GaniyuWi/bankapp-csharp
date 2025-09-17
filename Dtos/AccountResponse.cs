using Bank_API_EF.Models;

namespace Bank_API_EF.Dtos;

public record class AccountResponseDto
(
 long AccountNumber, 
 string FirstName,
 string LastName, 
 string PhoneNumber,
 string Email, 
 string Address, 
 string BVN, 
 string NIN,
 AccountType AccountType,
 decimal Balance,
 DateOnly CreatedAt
);
