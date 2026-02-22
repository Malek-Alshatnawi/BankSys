namespace BankSys.DTOs
{
    public class TopAccountDTO
    {
        public int id { get; set; }
        public String accountNumber { get; set; }
        public decimal balance { get; set; }
        public String accountType { get; set; }
        public String customerName { get; set; }
    }
}
