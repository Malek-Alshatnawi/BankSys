namespace BankSys.DTOs
{
    public class TransactionsInsideTheStatment
    {
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }

    }
}
