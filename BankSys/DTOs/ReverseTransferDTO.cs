using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class ReverseTransferDTO
    {
        public int ReverseFrom { set; get; }
        public int ReverseTo { set; get; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }


    }
}
