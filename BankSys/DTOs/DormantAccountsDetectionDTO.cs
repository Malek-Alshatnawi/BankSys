namespace BankSys.DTOs
{
    public class DormantAccountsDetectionDTO
    {
        public string accountnumber { get; set; }
        public string customerName { get; set; }
        public decimal balance { get; set; }
        public DateTime? lastTransactionDate { get; set; }
    }
}
