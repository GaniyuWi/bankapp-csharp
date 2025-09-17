using Bank_API_EF.Models;

namespace Bank_API_EF.Dtos;

public record class TransactionDto(
    TransactionType Type,
    decimal Amount,
    DateTime Date,
    decimal Balance,
    string? Description
);