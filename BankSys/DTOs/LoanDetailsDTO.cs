using BankSys.Models;

namespace BankSys.DTOs
{
    public class LoanDetailsDTO
    {
        public int LoanId { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public int TermMonths { get; set; }
        public List<InstallmentDTO> Installments { get; set; }
        public List<PaymentDTO> Payments { get; set; }

    }
    






}
