namespace BankSys.DTOs
{
    public class AccountsWithRecentMixedActivityDTO
    {

        public string accountnumber { get; set; }
        public string customerName { get; set; }
        public DateTime? lastCreditDate { get; set; }
        public DateTime? lastDebitDate { get; set; }
        public decimal balance { get; set; }
    }
}

