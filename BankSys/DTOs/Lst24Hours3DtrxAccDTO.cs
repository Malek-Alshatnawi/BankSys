namespace BankSys.DTOs
{
    public class Lst24Hours3DtrxAccDTO
    {
        public string AccountNumber { get; set; }

        public string customerName { get; set; }
        public List<Lst24Hours3DtrxAccLISTDTO> recentWithdrawals { get; set; }



    }
}
