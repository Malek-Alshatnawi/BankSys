using BankSys.DTOs;
using BankSys.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSys.Repositories
{
    public class CustomerRepository
    {
        MyDBContext db;

        public CustomerRepository (MyDBContext db)
        {
            this.db = db;
        }



        public async Task <List<GetCustomersWithHowManyAccountsTheyHaveDTO>> GetCustomersWithNumOfAcc()
        {

            var r = await db.Customers.Select(c => new GetCustomersWithHowManyAccountsTheyHaveDTO
            {
                id=c.Id,
                FullName=c.FullName,
                Email=c.Email,
                AccountsCount= c.Accounts.Count

            }).ToListAsync();

            return (r);
        }


        public async Task <GetCustomerByIdWithHisAccountsCountAndHistTotalBalanceInAllHisAccountsDTO>  GetCustomerAndAccCountAndTotalBalance(int id)
        {
            var r = await db.Customers.AsNoTracking().Where(c => c.Id == id).Select(c => new GetCustomerByIdWithHisAccountsCountAndHistTotalBalanceInAllHisAccountsDTO
            {
                Id=c.Id,
                FullName=c.FullName,
                NationalId=c.NationalId,
                Email=c.Email,
                Phone=c.Phone,
                AccountCount = c.Accounts.Count,
                TotalBalanceInAllAccounts = c.Accounts.Sum(a=> a.Balance)

            }).FirstOrDefaultAsync();
            
            return r;
        }



        public async Task<GetCustomerByIdWithHisAccountsCountAndHistTotalBalanceInAllHisAccountsDTO> GetCustomerAndAccCountAndTotalBalance(string name)
        {
            var r = await db.Customers.AsNoTracking().Where(c => c.FullName.Contains(name)).Select(c => new GetCustomerByIdWithHisAccountsCountAndHistTotalBalanceInAllHisAccountsDTO
            {
                Id = c.Id,
                FullName = c.FullName,
                NationalId = c.NationalId,
                Email = c.Email,
                Phone = c.Phone,
                AccountCount = c.Accounts.Count,
                TotalBalanceInAllAccounts = c.Accounts.Sum(a => a.Balance)

            }).FirstOrDefaultAsync();

            return r;
        }


        public async  Task <CustomerWithAccountsByIdDTO> GetCustomerWithAccountsByID (int Customerid)
        {
            var r = await db.Customers.AsNoTracking().Where(c => c.Id == Customerid).Select(c => new CustomerWithAccountsByIdDTO
            {
                CustomerId=c.Id,
                CustomerFullName=c.FullName,
                Accounts= c.Accounts.Select(a=> new AccountsInsideCustomersDTO
                {
                    AccountNumber=a.AccountNumber,
                    Balance=a.Balance,
                    AccountType=a.AccountType
                }).ToList()

            }).FirstOrDefaultAsync();

            return r;
        }




    }
}
