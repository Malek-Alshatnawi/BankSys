using System.ComponentModel.DataAnnotations.Schema;

namespace LoanSys.DTOs
{
    public class LoanApplicationResponse
    {
        public int LoanId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public String Status { get; set; }
    }
}
