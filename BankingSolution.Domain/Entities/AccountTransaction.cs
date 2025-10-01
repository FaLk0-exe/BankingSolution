using BankingSolution.Domain.Enums;

namespace BankingSolution.Domain.Entities;

public record AccountTransaction
{
    public Ulid Id { get; init; }
    public Account Account { get; init; }
    public decimal Amount { get; init; }    
    public AccountTransactionType Type { get; init; }

    // EF Core require parameterless constructor.
#pragma warning disable CS8618
    protected AccountTransaction()
    {
    }
    
    public AccountTransaction(Account account, decimal amount, AccountTransactionType type)
    {
        Id = Ulid.NewUlid();
        Account = account;
        Amount = amount;
        Type = type;
    }
}

public record ReplenishAccountTransaction : AccountTransaction
{
    public Account? AccountFrom { get; init; }

    // EF Core require parameterless constructor.
#pragma warning disable CS8618
    private ReplenishAccountTransaction()
    {
    }
    
    public ReplenishAccountTransaction(Account account, decimal amount, Account? accountFrom = null) : base(account, amount,
        AccountTransactionType.Replenish)
    {
        AccountFrom = accountFrom;
    }
}

public record WithdrawAccountTransaction : AccountTransaction
{
    public Account? AccountTo { get; init; }

    
    // EF Core require parameterless constructor.
#pragma warning disable CS8618
    private WithdrawAccountTransaction()
    {
        
    }
    
    public WithdrawAccountTransaction(Account account, decimal amount, Account? accountTo = null) : base(account, amount,
        AccountTransactionType.Withdraw)
    {
        AccountTo = accountTo;
    }
}