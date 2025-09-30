using BankingSolution.Domain.Entities;
using BankingSolution.Domain.Indicators;
using OneOf;

namespace BankingSolution.Domain.Services;

public class TransferService
{
    public OneOf<TransferSucceeded, IDomainError> TransferFunds(Account accountTo,
        Account accountFrom, decimal amount)
    {
        var withdrawResult = accountFrom.Withdraw(amount, accountTo);
        if (!withdrawResult.IsT0)
            return (OneOf<TransferSucceeded, IDomainError>)withdrawResult.Value;

        var replenishResult = accountTo.Replenish(amount, accountFrom);
        if (!replenishResult.IsT0)
            return (OneOf<TransferSucceeded, IDomainError>)replenishResult.Value;

        return new TransferSucceeded(accountFrom, accountTo,
            (accountFrom.AccountTransactions.Last(s =>
                s is WithdrawAccountTransaction) as WithdrawAccountTransaction)!);
    }
}