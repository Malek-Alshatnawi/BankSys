using BankSys.DTOs;
using BankSys.Enums;
using BankSys.Models;
using LoanSys.DTOs;
using LoanSys.ENUMs;
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
            await db.SaveChangesAsync();
            return ENUMs.LoanApplicationResults.TheLoanApplicationIsApproPending;
        }


        

        public async Task <ApproveLoanResults> ApproveLoan(int LoanId)
        {
            var loan = await db.Loans.FindAsync(LoanId);
            if (loan == null) return ApproveLoanResults.ThereIsNoLoanWithThisId;
            /* 
            The below and the above code are the same but the below code is more readable and easier to understand
            var CheckLoan = db.Loans.Any(l=>l.Id == LoanId);
            if (!CheckLoan)  return $"There is no loan with id {LoanId}";
            */

            if (loan.Status != "Pending") return ApproveLoanResults.ToApproveTheLoanTheStatusShouldBePending;



            await db.Loans.Where(l => l.Status == "Pending" && l.Id == LoanId).ExecuteUpdateAsync(e => e.SetProperty(l => l.Status, "Approved"));

            /*
            i can use the below instead of the above one as well      
            loan.Status = "Pending";
            await db.SaveChangesAsync();
            */


            var installmentAmount = loan.Amount / loan.TermMonths;
            var installemts = new List<LoanInstallment>();
            for (int i =1; i <= loan.TermMonths; i++)
            {
                installemts.Add(new LoanInstallment
                {
                    LoanId = loan.Id,
                    Amount = installmentAmount,
                    DueDate = DateTime.Now.AddMonths(i),
                    IsPaid = false
                });
            }

            await db.LoanInstallments.AddRangeAsync(installemts);
            await db.SaveChangesAsync();


            return ApproveLoanResults.TheLoanApprovedSuccessfully;


        }



        public async Task <LoanDetailsDTO> GetLoanByID (int loanid)
        {
            var r = await db.Loans.AsNoTracking().Where(l=> l.Id == loanid).Select(l=>  new LoanDetailsDTO
            {
                LoanId=l.Id,
                CustomerName=l.Customer.FullName,
                Amount=l.Amount,
                Status=l.Status,
                TermMonths=l.TermMonths,
                Installments = l.LoanInstallments.OrderByDescending(l=> l .DueDate).Select(i=> new InstallmentDTO
                {
                    Amount = i.Amount,
                    DueDate = i.DueDate,
                    IsPaid = i.IsPaid
                }).ToList(),
                Payments = l.LoanPayments.Select(p=> new PaymentDTO
                {
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                }).ToList(),

            }).FirstOrDefaultAsync();

            return r;
        }


        public async Task<PayInstallemtResults> PayInstallemt (MakepaymentRequest p)
        {
            var loan = await db.Loans.FindAsync(p.LoanId);
            if (loan == null) return PayInstallemtResults.LoanNotFound;
            var loaninstallment = await db.LoanInstallments.Where(i => i.LoanId == p.LoanId && !i.IsPaid).FirstOrDefaultAsync();
            if (loaninstallment == null) return PayInstallemtResults.ThereIsNoUnpaidInstallmetnsForThisLoan;
            if (loaninstallment.Amount != p.amount) return PayInstallemtResults.InvalidAmount;
            await db.LoanInstallments.Where(i=>i.LoanId == p.LoanId && !i.IsPaid).OrderBy(i=> i.DueDate).Take(1).ExecuteUpdateAsync(e => e.SetProperty(i => i.IsPaid, true));
            //loaninstallment.IsPaid = true;
             await db.LoanPayments.AddAsync(new LoanPayment
            {
                LoanId= p.LoanId,
                Amount= p.amount,
                PaidAt= DateTime.Now
            });
            await db.SaveChangesAsync();
            return PayInstallemtResults.Success;

        }



    }
}
