namespace Bank_API_EF.Dtos;

public record class TransferDto
(
    long FromAccountNumber,
    long ToAccountNumber,
    string FromPin,
    decimal Amount
);
