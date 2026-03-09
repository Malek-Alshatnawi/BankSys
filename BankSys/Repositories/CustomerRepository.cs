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

        public async Task <String> DeleteCustomerById(int customerid)
        {
            var r = db.Customers.Find(customerid);
            if (r == null) return "The mentioned customer id is not exist";
            var checkaccounts = db.Accounts.Any(a => a.CustomerId == customerid);
            if (checkaccounts) return "The mentioned customer id has accounts  can't be deleted";
            db.Customers.Remove(r);
            db.SaveChanges();
            return "The customer has been deleted successfully";
        }

        public async Task <String> CreateCustomer (CreateCustomerDTO cr)
        {
            Customer c = new Customer
            {
                FullName=cr.FullName,
                NationalId= cr.NationalId,
                Phone = cr.Phone,
                CreatedAt = DateTime.UtcNow,
                Email=cr.Email
            };

            await db.Customers.AddAsync(c);
            await db.SaveChangesAsync();
            return "The customer added succssffully";
        }

        public async Task <int> Importproducts()
        {
            HttpClient client = new HttpClient();
            var res = await client.GetFromJsonAsync<ExternalProductsResponse>("https://dummyjson.com/products");
            var productstable = await db.Products.ToListAsync();
            int addedCount =0;
            foreach (var p in res.Products)
            { 
            await db.Products.AddAsync(new Product
            {
                Name = p.Title,
                Price = p.Price,
                Brand = p.Brand,
                CreatedAt = DateTime.UtcNow,
                ExternalId = p.Id

            });
             await db.SaveChangesAsync();
                addedCount++;
            }
            return addedCount;
        }






    }
}
