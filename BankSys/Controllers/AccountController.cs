using BankSys.DTOs;
using BankSys.Enums;
using BankSys.Models;
using BankSys.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.Eventing.Reader;

namespace BankSys.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        AccountRepository repo;
        MyDBContext db;
        public AccountController(AccountRepository repo, MyDBContext db)
        {
            this.repo = repo;
            this.db = db;
        }


        [HttpGet]
        [SwaggerOperation(summary: "Here you can get the top 5 accounts with the highest balance", description: "")]
        public async Task<IActionResult> GetTopFiveAccounts()
        {
            var r = await repo.GetTopFiveAccounts();
            return Ok(new
            {
                success = true,
                message = "The top 5 accounts with the highest balance fetched successfully",
                data = r
            });
        }

        [HttpGet("{AccountNumber}")]
        [SwaggerOperation(summary: "Here you can get the account details by account number", description: "")]
        public async Task<IActionResult> GetAccountByAccNumber(string AccountNumber)
        {
            var r = await repo.GetAccountByAccNumber(AccountNumber);
            if (r == null) return NotFound($"There is no account with account number {AccountNumber}");
            else return Ok(new
            {
                success = true,
                message = "The account details fetched successfully",
                data = r
            });
        }


        [HttpPost]
        [SwaggerOperation(summary: "Here you can make a transfer from account to another", description: "please make sure that the both accounts are exist and the balance is enough")]
        public async Task<IActionResult> TransferbetweenAccounts(TransferRequestDTO tr)
        {
            if (tr == null) return BadRequest("Null reuqest, please fill the fields");

            var r = await repo.MakeTransfer(tr);
            if (r == TransferResults.TheAccountThatYouAreTransferFromIsNotExist) return BadRequest("The account you are trying to transfer from is not exist");
            if (r == TransferResults.TheAccountThatYouAreTransferToIsNotExist) return BadRequest("The account you are trying to transfer to is not exist");
            if (r == TransferResults.TheAmountShouldBeMoreThanZero) return BadRequest("The amount should be greater than zero");
            if (r == TransferResults.YouDontHaveInsuficiantbalance) return BadRequest("insuficiant balance!");
            if (r == TransferResults.TheSourceAccountAndDestinationAccountIsTheSame) return BadRequest("Source and Destination accoutns are same its not allowed !");
            if (r == TransferResults.TheTransferCompletedSuccessfully)
                return Ok(new
                {
                    success = true,
                    message = "The trasnfer completed successfully",
                    data = r

                });
            else return null;
        }


        [HttpGet("{AccountNumber}")]
        [SwaggerOperation(summary: "Here you can get the account statement", description: "please search by the account number")]
        public async Task<IActionResult> GetAccountStatements(String AccountNumber)
        {
            if (AccountNumber == null) return BadRequest("Null reuqest, please fill the fields");
            var r = await repo.GetAccountStatementByAccNumber(AccountNumber);
            if (r == null) return NotFound($"No account number with account number {AccountNumber}");
            return Ok(new
            {
                succes = true,
                message = "The account statement has been fetched successfully",
                data = r
            });

        }

        [HttpPost]
        [SwaggerOperation(summary: "Here you can make a deposit to your account", description: "please make sure that the account number is exist and the amount is greater than zero")]
        public async Task<IActionResult> Deposit(DepositAndWithdrawlDTO dw)
        {
            if (dw == null) return BadRequest("Null reuqest, please fill the fields");
            var r = await repo.DepositService(dw);
            if (r == DepositAndWithdrawlResults.TheprovidedAccountIsNotExist) return BadRequest("The account you are trying to deposit to is not exist");
            if (r == DepositAndWithdrawlResults.TheprovidedAmountShouldbeMoreThanZero) return BadRequest("The amount should be greater than zero");
            if (r == DepositAndWithdrawlResults.TheAccountIsFrozeenYouAreNotAllowedToPerformAnyTransaction) return BadRequest("The provided account is frozeen you are not allowed to perform any trasnactions");
            if (r == DepositAndWithdrawlResults.TheTransactionCompletedSuccessfully)
                return Ok(new
                {
                    success = true,
                    message = "The deposit completed successfully",
                    data = r
                });
            else return null;

        }


        [HttpPost]
        [SwaggerOperation(summary: "Here you can make a Withdraw from your account", description: "please make sure that the account number is exist and the amount is greater than zero")]
        public async Task<IActionResult> Withdraw(DepositAndWithdrawlDTO dw)
        {
            if (dw == null) return BadRequest("Null reuqest, please fill the fields");
            var r = await repo.WithdrawalService(dw);
            if (r == DepositAndWithdrawlResults.TheprovidedAccountIsNotExist) return BadRequest("The account you are trying to withdraw from is not exist");
            if (r == DepositAndWithdrawlResults.TheprovidedAmountShouldbeMoreThanZero) return BadRequest("The amount should be greater than zero");
            if (r == DepositAndWithdrawlResults.YouDonthavesufficiantbalance) return BadRequest("Insuficiant Balance");
            if (r == DepositAndWithdrawlResults.YouReachedTheDailyDebitLimitWith10K) return BadRequest("You Reached The Daily Debit Limit With 10K");
            if (r == DepositAndWithdrawlResults.TheTransactionCompletedSuccessfully)
                return Ok(new
                {
                    success = true,
                    message = "The Withdraw completed successfully",
                    data = r
                });
            else return null;

        }



        [HttpPost("{accountnumber}")]
        [SwaggerOperation(summary: "Here you can freeze an  account", description: "please make sure that the account number is exist")]
        public async Task<IActionResult> FreezeAccount(String accountnumber)
        {
            if (accountnumber == null) return BadRequest("Empty request please fill the data");
            var r = await repo.FreezeAccount(accountnumber);
            if (r == "The provided account number is not exist") return BadRequest("The provided account number is not exist");
            if (r == "The account is already frozen") return BadRequest("The account is already frozen");
            if (r == "The account has been frozen successfully") return Ok(new
            {
                success = true,
                message = "The account has been frozen successfully"
            });
            else return null;
        }


        [HttpPost("{accountnumber}")]
        [SwaggerOperation(summary: "Here you can activate the freezed account", description: "please make sure that the account number is exist")]
        public async Task<IActionResult> UnFreezeAccount(String accountnumber)
        {
            if (accountnumber == null) return BadRequest("Empty request please fill the data");
            var r = await repo.UnFreezeAccount(accountnumber);
            if (r == "The provided account number is not exist") return BadRequest("The provided account number is not exist");
            if (r == "The account is already active") return BadRequest("The account is already active");
            if (r == "The account has been activated successfully") return Ok(new
            {
                success = true,
                message = "The account has been activated successfully"
            });
            else return null;

        }





        [HttpPost("{customerid}")]
        [SwaggerOperation(summary: "Here you can atestttt", description: "please make sure that the account number is exist")]
        public async Task<IActionResult> testttt(int customerid)
        {
            //var since = DateTime.Now.AddHours(-24);
            //var r = await db.Accounts.Where(a => a.Transactions.Where(t => t.TransactionType == "Debit" && t.CreatedAt > since && t.Amount > 100).Count() > 3)
            //    .Select(a=> new Lst24Hours3DtrxAccDTO
            //    {
            //         AccountNumber= a.AccountNumber,
            //            customerName = a.Customer.FullName,
            //        recentWithdrawals = a.Transactions.Where(t => t.TransactionType == "Debit" && t.CreatedAt > since && t.Amount > 100).Select(t=> new Lst24Hours3DtrxAccLISTDTO
            //        {
            //            amount = t.Amount,
            //            date = t.CreatedAt
            //        }).ToList()

            //    }).ToListAsync();
            //return Ok(r);




            //if (customerid == 0) return BadRequest("Empty request please fill the data");
            //var r = await db.Transactions.AsNoTracking().Where(t=> t.Account.CustomerId == customerid).OrderByDescending(t=> t.CreatedAt).Take(15).
            //    Select(t=> new CrossAccountActivityFeedDTO
            //{
            //    accountNumber = t.Account.AccountNumber,
            //    type = t.TransactionType,
            //    amount = t.Amount,
            //    date = t.CreatedAt,
            //    description = t.Description

            //}).ToListAsync();
            //return Ok(r);


            //var since = DateTime.Now.AddDays(-90);
            //var r = await db.Accounts.Where(a => !a.IsFrozen && a.Balance > 0 && !a.Transactions.Any(t => t.CreatedAt >= since)).Select(a =>
            //    new DormantAccountsDetectionDTO
            //    {
            //        accountnumber = a.AccountNumber,
            //        customerName = a.Customer.FullName,
            //        balance = a.Balance,
            //        lastTransactionDate = a.Transactions.OrderByDescending(t => t.CreatedAt).Select(t => t.CreatedAt).FirstOrDefault()
            //    }
            //    ).ToListAsync();
            //return Ok(r);


            //var since = DateTime.Now.AddDays(-14);
            //var r = await db.Accounts.AsNoTracking().Where(
            //    a => !a.IsFrozen &&
            //    a.Transactions.Any(t => t.TransactionType == "Deposit" && t.CreatedAt > since) &&
            //    a.Transactions.Any(t => t.TransactionType == "Withdraw" && t.CreatedAt > since)
            //    ).Select(a => new AccountsWithRecentMixedActivityDTO
            //    {
            //        accountnumber = a.AccountNumber,
            //        customerName = a.Customer.FullName,
            //        lastCreditDate = a.Transactions.Where(t => t.TransactionType == "Deposit").OrderByDescending(t => t.CreatedAt).Select(t => t.CreatedAt).FirstOrDefault(),
            //        lastDebitDate = a.Transactions.Where(t => t.TransactionType == "Withdraw").OrderByDescending(t => t.CreatedAt).Select(t => t.CreatedAt).FirstOrDefault(),
            //        balance=a.Balance
            //    }
            //    ).ToListAsync();
            //return Ok(r);


            //var since = DateTime.Now.AddDays(-5);
            //var r = db.Accounts.AsNoTracking().Where(a =>
            //!a.IsFrozen &&
            //a.Transactions.Any(t1 => t1.TransactionType == "Deposit" && t1.CreatedAt > since &&
            //a.Transactions.Any(t2 => t2.TransactionType == "Withdraw" && t2.CreatedAt > since && t2.Amount == t1.Amount &&
            //EF.Functions.DateDiffMinute(t1.CreatedAt, t2.CreatedAt) <= 10 &&
            //t2.CreatedAt > t1.CreatedAt))).Select(/**/).TolistAsync();
            //return Ok(r);



           var since = DateTime.Now.AddDays(-20);
           var r = db.Accounts.AsNoTracking().Where(a =>
           !a.IsFrozen &&
           a.Transactions.Any(t1 => t1.TransactionType == "Deposit" && t1.CreatedAt > since 
           return Ok(r);




        }







    }

}
