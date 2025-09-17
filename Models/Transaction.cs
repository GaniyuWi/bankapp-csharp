namespace Bank_API_EF.Models
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut
    }

    public class Transaction
    {
        public int Id { get; set; }
        public long AccountNumber { get; set; }   // link to the account
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal Balance { get; set; }  // balance after the transaction
        public string? Description { get; set; }
    }
}