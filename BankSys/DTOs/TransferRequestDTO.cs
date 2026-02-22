using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class TransferRequestDTO
    {

        public string fromAccountNumber { get; set; }
        public string toAccountNumber { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }
    }
}
