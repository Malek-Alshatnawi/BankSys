using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class AccWithNoTrxInlast10Days
    {
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastTransactionDate { get; set; }
        
    }


    



}
