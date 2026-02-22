namespace BankSys.DTOs
{
    public class CustomerWithAccountsByIdDTO
    {

        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public List<AccountsInsideCustomersDTO> Accounts  { set;get;}
    }
}
 