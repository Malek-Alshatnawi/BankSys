namespace BankSys.DTOs
{
    public class GetOneAccountDTO
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
        public string AccountType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
