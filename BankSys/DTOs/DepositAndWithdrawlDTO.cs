using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class DepositAndWithdrawlDTO
    {
        public string AccountNumber { get; set; }
       
        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
    }
}
