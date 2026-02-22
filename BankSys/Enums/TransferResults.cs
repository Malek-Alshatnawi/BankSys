namespace BankSys.Enums
{
    public enum TransferResults
    {
        TheAccountThatYouAreTransferToIsNotExist,
        TheAccountThatYouAreTransferFromIsNotExist,
        TheSourceAccountAndDestinationAccountIsTheSame,
        TheAmountShouldBeMoreThanZero,
        YouDontHaveInsuficiantbalance,
        TheTransferCompletedSuccessfully,
        Null

    }
}
