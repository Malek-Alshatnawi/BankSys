using LoanSys.DTOs;
using LoanSys.ENUMs;
using LoanSys.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanSys.Repositories
{
    public class LoanRepo
    {
        MyDBContext db;
            public LoanRepo(MyDBContext _db)
        {
            db = _db;
        }


        public async Task <LoanApplicationResults> LoanApplication (LoanApplicationRequest l)
        {
            var CheckCustomer = await db.Customers.AnyAsync(c=> c.Id == l.CustomerId);
            if (!CheckCustomer) return ENUMs.LoanApplicationResults.TheprovidedCustomerIdIsNotExist;
            if (l.Amount > db.Customers.Where(c => c.Id == l.CustomerId).Select(c => c.MonthlyIncome).FirstOrDefault()*10 ) return ENUMs.LoanApplicationResults.Loanamountexceedsallowedlimit;

            /* The below and the above code are the same but the below code is more readable and easier to understand
            var customer = await db.Customers.FindAsync(l.CustomerId);
            if (l.Amount > customer.MonthlyIncome * 10) return ENUMs.LoanApplicationResults.Loanamountexceedsallowedlimit;
            */

            Loan nl = new Loan {
            Status = "Pending",
            InterestRate = 5,
            CreatedAt = DateTime.Now,
            CustomerId = l.CustomerId,
            Amount = l.Amount,
            TermMonths = l.TermMonths
             };

            db.Loans.Add(nl);
            db.SaveChangesAsync();
            return ENUMs.LoanApplicationResults.TheLoanApplicationIsApproved;


        }
    }
}
