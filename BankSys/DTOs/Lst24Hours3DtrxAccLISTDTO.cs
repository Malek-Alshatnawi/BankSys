using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class Lst24Hours3DtrxAccLISTDTO
    {

        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
        public DateTime? date { get; set; }
    }
}
