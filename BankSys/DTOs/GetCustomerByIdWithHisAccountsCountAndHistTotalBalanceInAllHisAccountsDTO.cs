using System.ComponentModel.DataAnnotations.Schema;

namespace BankSys.DTOs
{
    public class GetCustomerByIdWithHisAccountsCountAndHistTotalBalanceInAllHisAccountsDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int AccountCount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalBalanceInAllAccounts { get; set; }

    }
}
