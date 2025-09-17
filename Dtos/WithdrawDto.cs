namespace Bank_API_EF.Dtos;

public record class WithdrawDto
(
     long AccountNumber,
     string PinHash,
     decimal Amount
);
