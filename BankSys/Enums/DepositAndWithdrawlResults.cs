namespace BankSys.Enums
{
    public enum DepositAndWithdrawlResults
    {
        TheprovidedAccountIsNotExist,
        TheprovidedAmountShouldbeMoreThanZero,
        TheTransactionCompletedSuccessfully,
        YouDonthavesufficiantbalance,
        YouReachedTheDailyDebitLimitWith10K,
        TheAccountIsFrozeenYouAreNotAllowedToPerformAnyTransaction

    }
}
