using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class GetLastNTransactionsDTO
    {
        public String fromAccount { get; set; }
        public String toAccount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

    }

}