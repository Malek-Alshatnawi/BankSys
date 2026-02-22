using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class CrossAccountActivityFeedDTO
    {
        public string accountNumber { get; set; }
        public string type { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
        public DateTime? date { get; set; }
        public string description { get; set; }

    }
}



