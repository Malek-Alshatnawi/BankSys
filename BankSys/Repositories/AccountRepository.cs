using BankSys.DTOs;
using BankSys.Enums;
using BankSys.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net.WebSockets;

namespace BankSys.Repositories
{
    public class AccountRepository
    {

        MyDBContext db;
        public AccountRepository (MyDBContext db)
        {
            this.db = db;
        }

        public async Task<List<TopAccountDTO>> GetTopFiveAccounts()
        {
            var r = await db.Accounts.OrderByDescending(a=> a.Balance).Take(5).Select(a=> new TopAccountDTO
            {
                id = a.Id,
                accountNumber = a.AccountNumber,    
                balance = a.Balance,    
                accountType = a.AccountType,    
                customerName = a.Customer.FullName  
            }).ToListAsync();

            return r;
        }


        public async Task <GetOneAccountDTO> GetAccountByAccNumber (string AccountNumber)
        {
            var r = await db.Accounts.AsNoTracking().Where(a => a.AccountNumber == AccountNumber).Select(a=> new GetOneAccountDTO
            {
                Id=a.Id,
                AccountNumber=a.AccountNumber,
                CustomerId=a.CustomerId,
                Balance=a.Balance,
                AccountType=a.AccountType,
                IsActive=a.IsActive,
                CreatedAt=a.CreatedAt
            })
                .FirstOrDefaultAsync();
            return r;

        }


        public async Task<TransferResults> MakeTransfer(TransferRequestDTO tr)
        {
            var CheckSourceAcc = db.Accounts.Any(a => a.AccountNumber == tr.fromAccountNumber);
            if (!CheckSourceAcc) return TransferResults.TheAccountThatYouAreTransferFromIsNotExist;
            var CheckDestinationAccount = db.Accounts.Any(a => a.AccountNumber == tr.toAccountNumber);
            if (!CheckDestinationAccount) return TransferResults.TheAccountThatYouAreTransferToIsNotExist;
            if (tr.amount <= 0) return TransferResults.TheAmountShouldBeMoreThanZero;
            if (tr.fromAccountNumber == tr.toAccountNumber) return TransferResults.TheSourceAccountAndDestinationAccountIsTheSame;
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var TotalTransferAmount = await db.Transactions.Where(t => t.Account.AccountNumber == tr.fromAccountNumber && t.TransactionType == "Debit" && t.CreatedAt >= today && t.CreatedAt < tomorrow).SumAsync(t => t.Amount);
            if (TotalTransferAmount + tr.amount > 10000) return TransferResults.YouReachedTheDailyMaximumTransferLimit;
            else
            {
                using var transaction = await db.Database.BeginTransactionAsync();
                try
                {
                    var rowsAffected = await db.Accounts.Where(a => a.AccountNumber == tr.fromAccountNumber && a.Balance >= tr.amount).ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance - tr.amount));
                    if (rowsAffected == 0) return TransferResults.YouDontHaveInsuficiantbalance;
                    await db.Accounts.Where(a => a.AccountNumber == tr.toAccountNumber).ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance + tr.amount));


                    var fromAccount = await db.Accounts.FirstAsync(a => a.AccountNumber == tr.fromAccountNumber);
                    var toAccount = await db.Accounts.FirstAsync(a => a.AccountNumber == tr.toAccountNumber);

                    db.Transactions.Add(new Transaction
                    {
                        AccountId = fromAccount.Id,
                        TransactionType = "Debit",
                        Amount = tr.amount,
                        CreatedAt = DateTime.Now,
                        Description = $"Transfer to {tr.toAccountNumber}"
                    });

                    db.Transactions.Add(new Transaction
                    {
                        AccountId = toAccount.Id,
                        TransactionType = "Credit",
                        Amount = tr.amount,
                        CreatedAt = DateTime.Now,
                        Description = $"Transfer from {tr.fromAccountNumber}"
                    });

                    await transaction.CommitAsync();  
                    return TransferResults.TheTransferCompletedSuccessfully;

                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }




        public async Task<AccountStatementsDTO> GetAccountStatementByAccNumber (string AccountNumber)
        {
            var r = await db.Accounts.Where(a => a.AccountNumber == AccountNumber).Select(a => new AccountStatementsDTO
            {
                AccountNumber = a.AccountNumber,
                currentBalance = a.Balance,
                customerName = a.Customer.FullName,
                transactions = a.Transactions.Select(t=> new TransactionsInsideTheStatment
                {
                    TransactionType = t.TransactionType,
                    Amount = t.Amount,
                    Date = t.CreatedAt,
                    Description = t.Description
                }).ToList()

            }).FirstOrDefaultAsync();
            return r;
        }



        public async Task<DepositAndWithdrawlResults> DepositService (DepositAndWithdrawlDTO D)
        {
            var CheckAccount = await db.Accounts.AnyAsync(a => a.AccountNumber == D.AccountNumber);
            if (!CheckAccount) return DepositAndWithdrawlResults.TheprovidedAccountIsNotExist;
            if (D.amount < 0) return DepositAndWithdrawlResults.TheprovidedAmountShouldbeMoreThanZero;
            var account = await db.Accounts.Where(a => a.AccountNumber == D.AccountNumber).FirstOrDefaultAsync();
            if (account.IsFrozen) return DepositAndWithdrawlResults.TheAccountIsFrozeenYouAreNotAllowedToPerformAnyTransaction;
            await db.Accounts.Where(a => a.AccountNumber == D.AccountNumber && D.amount > 0).ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance + D.amount));
            db.Transactions.Add(new Transaction
            {
             AccountId = account.Id,
             Amount=D.amount,
             TransactionType = "Credit",
             CreatedAt = DateTime.Now,
             Description = "Deposit",
            });
            return DepositAndWithdrawlResults.TheTransactionCompletedSuccessfully;
        }

        

        public async Task<DepositAndWithdrawlResults> WithdrawalService (DepositAndWithdrawlDTO w)
        {
            var CheckAccount = await db.Accounts.AnyAsync(a => a.AccountNumber == w.AccountNumber);
            if (!CheckAccount) return DepositAndWithdrawlResults.TheprovidedAccountIsNotExist;
            if (w.amount <= 0) return DepositAndWithdrawlResults.TheprovidedAmountShouldbeMoreThanZero;
            var am = await db.Accounts.Where(a => a.AccountNumber == w.AccountNumber).FirstOrDefaultAsync();
            if (am.Balance < w.amount) return DepositAndWithdrawlResults.YouDonthavesufficiantbalance;
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var TotalDebitTransactionsAmount = await db.Transactions.Where(t => t.AccountId == am.Id && t.TransactionType == "Debit" && t.CreatedAt >= today && t.CreatedAt < tomorrow).SumAsync(t => t.Amount);
            if (w.amount + TotalDebitTransactionsAmount >= 10000) return DepositAndWithdrawlResults.YouReachedTheDailyDebitLimitWith10K;
            await db.Accounts.Where(a => a.AccountNumber == w.AccountNumber && w.amount < a.Balance).ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance - w.amount));
            var account = db.Accounts.Where(a => a.AccountNumber == w.AccountNumber).FirstOrDefault();
            db.Transactions.Add(new Transaction
            {
                AccountId= account.Id,
                Amount= w.amount,
                TransactionType= "Debit",
                Description = "Withdrawal",
                CreatedAt = DateTime.Now,
            });            
            return DepositAndWithdrawlResults.TheTransactionCompletedSuccessfully;
        }

        public async Task <String> FreezeAccount (String AccountNumber)
        {
            var checkaccount = await db.Accounts.AnyAsync(a => a.AccountNumber == AccountNumber);
            if (!checkaccount) return "The provided account number is not exist";
            var account = await db.Accounts.Where(a => a.AccountNumber == AccountNumber).FirstOrDefaultAsync();
            if (account.IsFrozen) return "The account is already frozen";
            await db.Accounts.Where(a => a.AccountNumber == AccountNumber).ExecuteUpdateAsync(s => s.SetProperty(a => a.IsFrozen, true));
            return "The account has been frozen successfully";
        }


        public async Task<String> UnFreezeAccount(String AccountNumber)
        {
            var checkaccount = await db.Accounts.AnyAsync(a => a.AccountNumber == AccountNumber);
            if (!checkaccount) return "The provided account number is not exist";
            var account = await db.Accounts.Where(a => a.AccountNumber == AccountNumber).FirstOrDefaultAsync();
            if (!account.IsFrozen) return "The account is already active";
            await db.Accounts.Where(a => a.AccountNumber == AccountNumber).ExecuteUpdateAsync(s => s.SetProperty(a => a.IsFrozen, false));
            return "The account has been activated successfully";
        }




        public async Task<string> CreateAccount(CreateAccountDTO cdt)
        {
            var CheckAccNumber = await db.Accounts.AnyAsync(a => a.AccountNumber == cdt.AccountNumber);
            if (CheckAccNumber) return "The Account number is already exist";
            var ChechIfTheCustomerIdExist = await db.Customers.AnyAsync(c => c.Id == cdt.CustomerId);
            if (!ChechIfTheCustomerIdExist) return "The provided customer id is not exist";
            if (cdt.Balance <= 0) return "The balance should me more than zero";
          
                Account a = new Account()
                {
                    AccountNumber = cdt.AccountNumber,
                    Balance = cdt.Balance,
                    CustomerId=cdt.CustomerId,
                    AccountType = cdt.AccountType,
                    IsActive = cdt.IsActive
                };
            db.Accounts.Add(a);
            db.SaveChanges();
            return "The account has been created successfully"; 
        }

        public async Task <String> CloseAccount (int AccountId)
        {
            var checkAccount = await db.Accounts.FindAsync(AccountId);
            if (checkAccount == null) return "The account id is not esixt";
            if (checkAccount.IsFrozen) return "The account is already frozen";
            if (checkAccount.Balance != 0) return "The account balance should be zero";
            else
            {
                await db.Accounts.Where(a=> a.Id == AccountId).ExecuteUpdateAsync(e => e.SetProperty(a => a.IsFrozen, true));
                return "The account has been closed succsessfully";
            }
        }

        public async Task<String> UpdateAccountType (String AccountType, int accountid)
        {
            var account = await db.Accounts.FindAsync(accountid);
            if (account.AccountType == AccountType) return "The account type is already exist";
            //await db.Accounts.Where(a => a.Id == accountid).ExecuteUpdateAsync(s => s.SetProperty(a=> a.AccountType, AccountType));
            account.AccountType = AccountType;
            await db.SaveChangesAsync();
            return "The account Type has been canged successfully";
        }

        public async Task <List<AccWithNoTrxInlast10Days>> GetAccWithNoTrxInlast10Days()
        {
            var since = DateTime.Today.AddDays(-10);
            var res = db.Accounts.AsNoTracking().Where(a => !a.Transactions.Any(t => t.CreatedAt > since)).Select(a => new AccWithNoTrxInlast10Days
            {
                AccountNumber = a.AccountNumber,
                CustomerName = a.Customer.FullName,
                Balance = a.Balance,
                LastTransactionDate = a.Transactions.OrderByDescending(t => t.CreatedAt).Select(t => t.CreatedAt).FirstOrDefault()
            }
            ).OrderByDescending(a=>a.Balance).ToList();
            return res;
        }




    }
}
