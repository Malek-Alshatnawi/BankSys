using System.ComponentModel.DataAnnotations;

namespace BankSys.DTOs
{
    public class CreateCustomerDTO
    {
        [Required]
        [StringLength(200)]
        public string FullName { get; set; }
        [Required]
        [StringLength(20)]
        public string NationalId { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }

    }
}
