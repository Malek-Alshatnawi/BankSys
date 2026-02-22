using BankSys.Models;

namespace BankSys.Repositories
{
    public class TransferRepository
    {
        MyDBContext db;

        public TransferRepository (MyDBContext db)
        {
            this.db = db;
        }


    }
}
