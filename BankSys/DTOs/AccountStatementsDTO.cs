namespace BankSys.DTOs
{
    public class AccountStatementsDTO
    {

        public string AccountNumber { get; set; }
        public decimal currentBalance { get; set; }
        public string customerName { get; set; }
        public List<TransactionsInsideTheStatment> transactions { get; set; }

    }
}
