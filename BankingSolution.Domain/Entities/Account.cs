using BankingSolution.Domain.Indicators;
using OneOf;

namespace BankingSolution.Domain.Entities;

public record Account
{
    private readonly List<AccountTransaction> _accountTransactions = new();
    public Ulid Id { get; init; }
    public User User { get; init; }

    public IReadOnlyList<AccountTransaction> AccountTransactions => _accountTransactions;

    //Also we could implement BalanceSnapshot for a future optimization
    public Decimal Balance => AccountTransactions.Sum(s => s.Amount);


    // EF Core require parameterless constructor.
#pragma warning disable CS8618
    private Account()
    {
    }

    private Account(User user)
    {
        Id = Ulid.NewUlid();
        User = user;
    }

    private Account(User user, decimal initialAmount) : this(user)
    {
        Replenish(initialAmount);
    }

    public static OneOf<Account, IDomainError> Create(User user, decimal initialAmount)
    {
        if (!IsValidInitialReplenishFundAmount(initialAmount))
            return new InvalidOperationAmount();

        if (initialAmount == 0)
            return new Account(user);

        return new Account(user, initialAmount);
    }

    public OneOf<AccountReplenishSucceeded, IDomainError> Replenish(decimal amount,
        Account? accountFrom = null)
    {
        if (accountFrom is not null && IsSelfTransfer(accountFrom))
            return new SelfTransfer(this);

        if (!IsValidOperationFundAmount(amount))
            return new InvalidOperationAmount(this, amount);

        var replenishTransaction = new ReplenishAccountTransaction(this, amount, accountFrom);
        _accountTransactions.Add(replenishTransaction);

        return new AccountReplenishSucceeded(replenishTransaction);
    }

    public OneOf<AccountWithdrawSucceeded, IDomainError> Withdraw(decimal amount,
        Account? accountTo = null)
    {
        if (!IsValidOperationFundAmount(amount))
            return new InvalidOperationAmount(this, amount);
        
        if (accountTo is not null && IsSelfTransfer(accountTo))
            return new SelfTransfer(this);
        
        if (Balance - amount < 0)
            return new EnoughFunds(this, amount);

        var withDrawTransaction = new WithdrawAccountTransaction(this, amount * -1, accountTo);
        
        _accountTransactions.Add(withDrawTransaction);
        
        return new AccountWithdrawSucceeded(withDrawTransaction);
    }
    
    private bool IsValidOperationFundAmount(decimal amount) => amount > 0;
    private static bool IsValidInitialReplenishFundAmount(decimal amount) => amount >= 0;
    private bool IsSelfTransfer(Account account) => account == this;

}