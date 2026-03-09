using BankSys.Models;

namespace BankSys.DTOs
{
    public class InstallmentDTO
    {
       
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }


    }
}
