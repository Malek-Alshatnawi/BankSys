namespace BankSys.DTOs
{
    public class GetCustomersWithHowManyAccountsTheyHaveDTO
    {

        public int id { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public int AccountsCount { get; set; }
        
    }
}
