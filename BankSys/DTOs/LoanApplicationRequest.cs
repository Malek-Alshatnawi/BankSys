using System.ComponentModel.DataAnnotations.Schema;

namespace LoanSys.DTOs
{
    public class LoanApplicationRequest
    {
        public int CustomerId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public int TermMonths { get; set; }


    }
}
