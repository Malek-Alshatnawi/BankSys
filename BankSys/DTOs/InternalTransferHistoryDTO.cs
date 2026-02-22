namespace BankSys.DTOs
{
    public class InternalTransferHistoryDTO
    {
        public String type { get; set; }
        public decimal amount { get; set; }
        public DateTime? date { get; set; }

        public String counterPartyAccount { get; set; }
    }
}
