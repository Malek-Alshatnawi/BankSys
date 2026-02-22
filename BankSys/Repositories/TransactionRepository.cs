using BankSys.Models;

namespace BankSys.Repositories
{
    public class TransactionRepository
    {
        MyDBContext db;

        public TransactionRepository (MyDBContext db)
        {
            this.db = db;
        }


    }
}
