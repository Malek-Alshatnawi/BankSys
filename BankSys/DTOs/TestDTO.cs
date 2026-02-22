using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class TestDTO
    {

        public string customerName { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal totalBalance { get; set; }

        public int activeAccountsCount { get; set; }

        public int recentTransactionCount { get; set; }


    }
}
