namespace BankSys.DTOs
{
    public class CustomerHybridDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public ExternalProfileDTO ExternalProfile { get; set; }
    }
}
