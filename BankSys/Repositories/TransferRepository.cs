using BankSys.DTOs;
using BankSys.Enums;
using BankSys.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BankSys.Repositories
{
    public class TransferRepository
    {
        MyDBContext db;

        public TransferRepository (MyDBContext db)
        {
            this.db = db;
        }



        public async Task<ReverseTransferResults> ReverseATransfer(ReverseTransferDTO rt)
        {
            var r = await db.Transfers.AnyAsync
                (t => t.FromAccountId == rt.ReverseTo && t.ToAccountId == rt.ReverseFrom && t.Amount == rt.Amount);
            if (!r) return ReverseTransferResults.ThereIsNoTransferWithTheprovidedDetailes;
            var CheckAccountBalance = await db.Accounts.Where(a => a.Id == rt.ReverseFrom).Select(a=> a.Balance).FirstOrDefaultAsync();
            if (CheckAccountBalance < rt.Amount) return ReverseTransferResults.InsufciantBalance;


            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {

                var roweffected = await db.Accounts.Where(a => a.Id == rt.ReverseFrom && a.Balance >= rt.Amount).ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance - rt.Amount));
                var roweffected2 = await db.Accounts.Where(a => a.Id == rt.ReverseTo).ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance + rt.Amount));

                if (roweffected == 0 || roweffected2 == 0)
                {
                    await transaction.RollbackAsync();
                    return ReverseTransferResults.InsufciantBalance;
                }

                db.Transactions.Add(new Transaction
                {
                    AccountId = rt.ReverseFrom,
                    Amount = rt.Amount,
                    CreatedAt = DateTime.UtcNow,
                    TransactionType = "ReverseWithdraw",
                    Description = "ReversalFromThisAccToAnother"
                }
                );
                db.Transactions.Add(new Transaction
                {
                    AccountId = rt.ReverseTo,
                    Amount = rt.Amount,
                    CreatedAt = DateTime.UtcNow,
                    TransactionType = "ReverseDeposit",
                    Description = "ReversalFromThisAccIntoAnotherAcc"
                });

                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                return ReverseTransferResults.TheReversalCompletedSuccessfully;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }


    }
}
