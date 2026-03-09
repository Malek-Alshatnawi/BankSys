using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class CreateAccountDTO
    {

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountType { get; set; }
        public bool? IsActive { get; set; }
 
    }
}
