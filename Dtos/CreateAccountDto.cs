using System.Security.Cryptography.X509Certificates;
using Bank_API_EF.Models;

namespace Bank_API_EF.Dtos;

public record class CreateAccountDto
(
     string FirstName, 
     string LastName, 
     decimal Balance,
     string Pin, 
     string Email, 
     string Address, 
     string BVN, 
     string NIN,
     string PhoneNumber,
     long AccountNumber,
     AccountType AccountType,
     DateOnly CreatedAt
);
